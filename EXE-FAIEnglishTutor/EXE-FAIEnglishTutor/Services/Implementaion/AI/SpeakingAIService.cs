    using EXE_FAIEnglishTutor.Services.Interface.AI;
    using System.Collections.Generic;
    using System.Text.Json;

    namespace EXE_FAIEnglishTutor.Services.Implementaion.AI
    {
        public class SpeakingAIService : ISpeakingAIService
        {
            private readonly IOpenAIService _openAIClient;

            public SpeakingAIService(IOpenAIService openAIClient)
            {
                _openAIClient = openAIClient;
            }

            public async Task<string> GetChatResponseAsync(object messages)
            {
                // Serialize với CamelCase policy → keys luôn là lowercase (role, content)
                // dù input là PascalCase class (Role/Content) hay anonymous type (role/content)
                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var json = JsonSerializer.Serialize(messages, serializeOptions);
                var messagesDict = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(json);

                return await _openAIClient.CallGeminiAsync(messagesDict);
            }

            public async Task<string> TranscribeAudioAsync(byte[] audioBytes)
            {
                return await _openAIClient.TranscribeAudioAsync(audioBytes);
            }
        }
    }
