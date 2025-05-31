using EXE_FAIEnglishTutor.Services.Implimentaion.AI;
using EXE_FAIEnglishTutor.Services.Interface.AI;
using System.Net.Http;
using System.Text;

namespace EXE_FAIEnglishTutor.Services.Implementaion.AI
{
    public class SpeakingAIService :  ISpeakingAIService
    {
        private readonly IOpenAIService _openAIClient;

        public SpeakingAIService(IOpenAIService openAIClient)
        {
            _openAIClient = openAIClient;
        }

        public async Task<string> GetChatResponseAsync(string userMessage, string situation = null)
        {
            var systemPrompt = situation != null
                ? $"{situation}\nProvide a concise, natural, and conversational response in English suitable for speaking practice. Do not summarize the situation, just respond naturally as the specified role."
                : "You are a helpful assistant. Provide a concise and conversational response in English suitable for speaking practice.";

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userMessage }
                },
                max_tokens = 100
            };

            return await _openAIClient.CallOpenAIAsync(requestBody);
        }

        public async Task<string> TranscribeAudioAsync(byte[] audioBytes)
        {
            return await _openAIClient.TranscribeAudioAsync(audioBytes);
        }
    }
}
