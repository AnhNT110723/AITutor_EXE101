using System.Collections.Generic;
using System.Threading.Tasks;

namespace EXE_FAIEnglishTutor.Services.Interface.AI
{
    public interface IAIService
    {
        /// <summary>
        /// Tạo văn bản trả lời dựa trên một câu prompt đơn giản (dùng cho Reading, Grammar, Vocabulary).
        /// create by ntanh - 21/08/2026
        /// </summary>
        Task<string> GenerateTextAsync(string prompt);

        /// <summary>
        /// Tạo câu trả lời hội thoại dựa trên danh sách tin nhắn lịch sử (hỗ trợ role system, user, assistant/model).
        /// create by ntanh - 21/08/2026
        /// </summary>
        Task<string> GenerateChatResponseAsync(List<Dictionary<string, string>> messages);

        /// <summary>
        /// Nhận diện và chuyển đổi tệp âm thanh giọng nói thành văn bản (Speech-to-Text).
        /// create by ntanh - 21/08/2026
        /// </summary>
        Task<string> TranscribeAudioAsync(byte[] audioBytes);
    }
}
