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

            public async Task<string> GetChatResponseAsync(object messages)
            {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = messages,
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
