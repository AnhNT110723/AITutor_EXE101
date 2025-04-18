using EXE_FAIEnglishTutor.Services.Interface;

namespace EXE_FAIEnglishTutor.Services.Implementaion
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileUploadService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<string> UploadFileAsync(IFormFile file, string folder = "uploads")
        {
            if (file == null || file.Length == 0)
            {
                return null; // Trả về null nếu không có file
            }

            // Tạo đường dẫn thư mục lưu file trong wwwroot
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder); // Tạo thư mục nếu chưa tồn tại
            }

            // Tạo tên file duy nhất
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Lưu file vào server
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Trả về đường dẫn tương đối
            return $"/{folder}/{uniqueFileName}";
        }
    }
}
