    using EXE_FAIEnglishTutor.Services.Interface.AI;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;

    namespace EXE_FAIEnglishTutor.Services.Implementaion.AI
    {
        public class SpeakingAIService : ISpeakingAIService
        {
            private readonly IAIService _aiService;

            public SpeakingAIService(IAIService aiService)
            {
                _aiService = aiService;
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

                return await _aiService.GenerateChatResponseAsync(messagesDict);
            }

            public async Task<string> TranscribeAudioAsync(byte[] audioBytes)
            {
                return await _aiService.TranscribeAudioAsync(audioBytes);
            }
        }
    }
