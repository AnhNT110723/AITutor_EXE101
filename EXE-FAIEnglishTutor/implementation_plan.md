# Triển khai tính năng tạo tình huống cá nhân và gửi ảnh trong Chat

Kế hoạch này bao gồm hai tính năng chính mà bạn yêu cầu: Thêm chức năng tải ảnh (và xóa tình huống) cho phần tạo tình huống, và cho phép dán ảnh vào chatbot.

## User Review Required

> [!WARNING]
> **Cập nhật Database (Entity Framework):**
> Để chức năng "người dùng nào tạo thì người ấy có thể xóa" hoạt động, chúng ta cần biết ai là người tạo tình huống đó. Hiện tại, bảng `Situation` không có cột lưu ID của người dùng. 
> Tôi sẽ thêm cột `CreatedByUserId` vào model `Situation` và chạy Entity Framework Migrations (`dotnet ef migrations add ...` và `dotnet ef database update`) để cập nhật CSDL. Điều này có an toàn trong môi trường hiện tại của bạn không?

> [!IMPORTANT]
> **Tích hợp ảnh vào Chatbot:**
> Chatbot của bạn hiện đang dùng `OpenAIService.CallGeminiAsync`. Để chatbot hiểu được ảnh (nhận diện hình ảnh), tôi sẽ cập nhật hàm này để gửi dữ liệu ảnh (chuỗi Base64) lên Gemini Vision API. Bạn có đồng ý lưu trữ ảnh dán vào dưới dạng Base64 tạm thời trong bộ nhớ để gửi cho AI thay vì lưu thành file cứng vào thư mục không (giúp tiết kiệm dung lượng ổ cứng)?

## Proposed Changes

---

### 1. Model & Database
Cập nhật các Entities để liên kết tình huống với người dùng.

#### [MODIFY] [Situation.cs](file:///d:/Capstones/New%20folder/AITutor_EXE101/EXE-FAIEnglishTutor/EXE-FAIEnglishTutor/Models/Situation.cs)
- Thêm thuộc tính `CreatedByUserId` (int?).
- Thêm Navigation property `CreatedByUser`.

#### [MODIFY] [User.cs](file:///d:/Capstones/New%20folder/AITutor_EXE101/EXE-FAIEnglishTutor/EXE-FAIEnglishTutor/Models/User.cs)
- Thêm tập hợp `Situations` (ICollection<Situation>) để thể hiện mối quan hệ 1-N.

#### [MODIFY] [FaiEnglishContext.cs](file:///d:/Capstones/New%20folder/AITutor_EXE101/EXE-FAIEnglishTutor/EXE-FAIEnglishTutor/Models/FaiEnglishContext.cs)
- Định nghĩa khóa ngoại cho `CreatedByUserId` trong hàm `OnModelCreating` (nếu cần).

---

### 2. Giao diện Role-Play (Tạo & Xóa tình huống)

#### [MODIFY] [ListSituations.cshtml](file:///d:/Capstones/New%20folder/AITutor_EXE101/EXE-FAIEnglishTutor/EXE-FAIEnglishTutor/Areas/Mentee/Views/SpekingAiSituation/ListSituations.cshtml)
- Thêm trường `<input type="file" id="situationImage" accept="image/*">` vào form "Tạo tình huống".
- Bổ sung JS để Validate ảnh ở client:
  - Kiểm tra định dạng (jpg, png, webp...).
  - Kiểm tra dung lượng (vd: tối đa 5MB).
  - Kiểm tra ảnh hợp lệ.
- Bổ sung logic JavaScript gửi dữ liệu qua `FormData` bằng AJAX.
- Hiển thị nút "Xóa" trên các `card` tình huống nếu ID của người đăng nhập trùng khớp với `CreatedByUserId`.
- Gắn sự kiện gọi API xóa tình huống qua AJAX.

#### [MODIFY] [SpekingAiSituationController.cs](file:///d:/Capstones/New%20folder/AITutor_EXE101/EXE-FAIEnglishTutor/EXE-FAIEnglishTutor/Areas/Mentee/Controllers/SpekingAiSituationController.cs)
- Bổ sung endpoint `[HttpPost] CreateSituation(...)` nhận dữ liệu form và file ảnh, lưu ảnh vào thư mục `wwwroot/Images/` và lưu thông tin vào DB.
- Bổ sung endpoint `[HttpDelete] DeleteSituation(int id)` xử lý quyền xóa tình huống.

---

### 3. Chatbot - Dán ảnh vào chat

#### [MODIFY] [ai-chat-utils.js](file:///d:/Capstones/New%20folder/AITutor_EXE101/EXE-FAIEnglishTutor/EXE-FAIEnglishTutor/wwwroot/js/ai-chat-utils.js) (hoặc file js riêng của chat)
- Lắng nghe sự kiện `paste` trên khung nhập chat.
- Khi người dùng `Ctrl+V` một bức ảnh, đọc dữ liệu ảnh, chuyển sang dạng chuỗi Base64 và hiển thị một khung preview nhỏ trên ô chat để người dùng thấy ảnh sắp gửi.
- Sửa hàm gửi AJAX để đính kèm chuỗi Base64 cùng với tin nhắn text.

#### [MODIFY] ChatController.cs
- Thay đổi hàm `SendMessage` để có thể nhận thêm thuộc tính `imageBase64`.

#### [MODIFY] [OpenAIService.cs](file:///d:/Capstones/New%20folder/AITutor_EXE101/EXE-FAIEnglishTutor/EXE-FAIEnglishTutor/Services/Implementaion/AI/OpenAIService.cs) & [SpeakingAIService.cs](file:///d:/Capstones/New%20folder/AITutor_EXE101/EXE-FAIEnglishTutor/EXE-FAIEnglishTutor/Services/Implementaion/AI/SpeakingAIService.cs)
- Cập nhật cách parse dữ liệu truyền vào `CallGeminiAsync`.
- Format lại chuỗi JSON cho Gemini API để nếu có ảnh, nó sẽ đẩy ảnh vào mảng `parts` dưới định dạng `inlineData` (Base64) để Gemini có thể "nhìn" thấy ảnh.

## Verification Plan

### Manual Verification
1. Đăng nhập và vào mục "Role-Play". Nhấn tạo tình huống, không nhập ảnh hoặc nhập file sai định dạng để kiểm tra Validate.
2. Chọn một ảnh thật, điền thông tin và "Gửi". Xem tình huống mới có hiển thị cùng ảnh đúng không.
3. Kiểm tra nút Xóa có hiện ở tình huống vừa tạo không, thử click Xóa và tải lại trang. Đăng xuất và đăng nhập nick khác xem nút Xóa có ẩn không.
4. Mở cửa sổ Chat, copy một ảnh từ Snipping Tool hoặc web, dán (Ctrl+V) vào ô nhập chữ. Xem ảnh có hiện preview không. Gõ thêm text "Ảnh này có gì" và nhấn gửi để test phản hồi của AI.
