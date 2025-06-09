namespace EXE_FAIEnglishTutor.Services.Interface.AI
{
    public interface ISpeakingAIService
    {
        Task<string> GetChatResponseAsync(object messages);
        Task<string> TranscribeAudioAsync(byte[] audioBytes);
    }
}
