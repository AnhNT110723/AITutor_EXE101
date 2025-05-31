namespace EXE_FAIEnglishTutor.Services.Interface.AI
{
    public interface IOpenAIService
    {
        Task<string> CallOpenAIAsync(object requestBody);
        Task<string> TranscribeAudioAsync(byte[] audioBytes);
    }
}
