using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Services.Interface.AI
{
    public interface IOpenAIService
    {
        Task<string> CallOpenAIAsync(object requestBody);
        Task<string> CallGeminiAsync(List<Dictionary<string, string>> messages);
        Task<string> TranscribeAudioAsync(byte[] audioBytes);
    }
}
