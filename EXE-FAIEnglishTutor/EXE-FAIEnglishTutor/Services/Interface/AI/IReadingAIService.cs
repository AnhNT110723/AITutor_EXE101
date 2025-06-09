namespace EXE_FAIEnglishTutor.Services.Interface.AI
{
    public interface IReadingAIService
    {
        Task<string> GenerateReadingPassageAsync(string level, string levelScore);
        Task<string> ExplainVocabularyAsync(string word, string context);
        Task<string> ExplainGrammarAsync(string sentence);
        Task<string> SummarizePassageAsync(string passage);
    }
}
