using EXE_FAIEnglishTutor.Services.Implimentaion.AI;
using EXE_FAIEnglishTutor.Services.Interface.AI;

namespace EXE_FAIEnglishTutor.Services.Implementaion.AI
{
    public class ReadingAIService : IReadingAIService
    {
        private readonly IOpenAIService _openAIClient;

        public ReadingAIService(IOpenAIService openAIClient)
        {
            _openAIClient = openAIClient;
        }
        public async Task<string> GenerateReadingPassageAsync(string level, string levelScore)
        {
            var prompt = $"Create a reading passage suitable for {level} level (CEFR) equivalent to {levelScore} TOEIC. The passage should be engaging, appropriate for language learners, and between 100-200 words.";
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 300
            };

            return await _openAIClient.CallOpenAIAsync(requestBody);
        }
        public async Task<string> ExplainGrammarAsync(string sentence)
        {
            var prompt = $"Analyze the grammar structure of the sentence: '{sentence}'. Explain the grammar rules and usage clearly in a way that is easy for language learners to understand.";
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 300
            };

            return await _openAIClient.CallOpenAIAsync(requestBody);
        }

        public async Task<string> ExplainVocabularyAsync(string word, string context)
        {
            var prompt = $"Explain the meaning, pronunciation, and usage of the word '{word}' in the context: '{context}'. Provide a clear definition, phonetic pronunciation, and 1-2 example sentences.";
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 200
            };

            return await _openAIClient.CallOpenAIAsync(requestBody);
        }



        public async Task<string> SummarizePassageAsync(string passage)
        {
            var prompt = $"Summarize the following passage in 2-3 sentences, keeping it clear and concise for language learners: '{passage}'";
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 150
            };

            return await _openAIClient.CallOpenAIAsync(requestBody);
        }
    }
}
