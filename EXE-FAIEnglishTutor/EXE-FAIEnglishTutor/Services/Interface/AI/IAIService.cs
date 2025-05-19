namespace EXE_FAIEnglishTutor.Services.Interface.AI
{
    public interface IAIService
    {
        Task<string> GetChatResponseAsync(string userMessage, string situation = null);
        Task<string> TranscribeAudioAsync(byte[] audioBytes);
    }
}
