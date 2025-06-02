namespace EXE_FAIEnglishTutor.Services.Interface.AI
{
    public interface ISpeakingAIService
    {
        Task<string> GetChatResponseAsync(string userMessage, string situation = null);
        Task<string> TranscribeAudioAsync(byte[] audioBytes);
    }
}
