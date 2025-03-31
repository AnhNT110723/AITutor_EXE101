namespace EXE_FAIEnglishTutor.Services.Interface.AI
{
    public interface IChatBotService
    {
        Task<string> GetChatResponseAsync(string userMessage);
    }
}
