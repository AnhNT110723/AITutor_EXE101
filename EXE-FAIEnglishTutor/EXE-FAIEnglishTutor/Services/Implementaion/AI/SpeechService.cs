using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Services.Interface.AI;
using System.Text;
using System.Text.Json;

namespace EXE_FAIEnglishTutor.Services.Implementaion.AI
{
    public class SpeechService : ISpeechService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public SpeechService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
        }
        public async Task<string> GeneratePostcardAsync(string prompt)
        {
            try
            {
                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                    new { role = "user", content = prompt }
                },
                    max_tokens = 500,
                    temperature = 0.7
                };

                var response = await _httpClient.PostAsJsonAsync("chat/completions", requestBody);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"OpenAI API Error: {response.StatusCode} - {errorContent}");
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(jsonResponse);
                var content = jsonDoc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                if (string.IsNullOrWhiteSpace(content))
                {
                    Console.WriteLine("Error: Empty content received from API.");
                    return null;
                }

                return content;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Parsing Error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
                return null;
            }
        }

        public async Task<string> TranscribeAudioAsync(string audioPath, string language = "en")
        {
            using var form = new MultipartFormDataContent();
            using var fileContent = new StreamContent(File.OpenRead(audioPath));
            form.Add(fileContent, "file", Path.GetFileName(audioPath));
            form.Add(new StringContent("whisper-1"), "model");
            form.Add(new StringContent(language), "language"); // Set to "vi" for Vietnamese

            var response = await _httpClient.PostAsync("audio/transcriptions", form);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<WhisperResponse>();
            return result?.Text?.Trim() ?? string.Empty;
        }

        public double CalculateLevenshteinScore(string source, string target)
        {
            int[,] matrix = new int[source.Length + 1, target.Length + 1];

            for (int i = 0; i <= source.Length; i++)
                matrix[i, 0] = i;
            for (int j = 0; j <= target.Length; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= source.Length; i++)
            {
                for (int j = 1; j <= target.Length; j++)
                {
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }

            int distance = matrix[source.Length, target.Length];
            return Math.Max(0, 100 - (distance / (double)Math.Max(source.Length, 1)) * 100);
        }

        public double CalculateSequenceMatchScore(string source, string target)
        {
            var matches = 0;
            var total = Math.Max(source.Length, target.Length);

            for (int i = 0; i < Math.Min(source.Length, target.Length); i++)
            {
                if (source[i] == target[i])
                    matches++;
            }

            return (matches / (double)total) * 100;
        }

        public int CountSyllables(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            text = text.ToLower();
            int syllableCount = 0;
            string vowels = "aeiouy";
            bool prevCharWasVowel = false;

            foreach (var word in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                int wordSyllables = 0;
                foreach (char c in word)
                {
                    if (vowels.Contains(c) && !prevCharWasVowel)
                    {
                        wordSyllables++;
                        prevCharWasVowel = true;
                    }
                    else if (!vowels.Contains(c))
                    {
                        prevCharWasVowel = false;
                    }
                }
                if (word.EndsWith("e"))
                    wordSyllables = Math.Max(1, wordSyllables - 1); // Handle silent 'e'
                syllableCount += wordSyllables;
            }

            return syllableCount;
        }

        private class WhisperResponse
        {
            public string Text { get; set; }
        }
        public async Task TextToSpeechAsync(string text, string outputPath, string language)
        {
            var ttsModel = "tts-1"; // OpenAI TTS model
            var voice = "alloy"; // Suitable for English pronunciation

            var requestBody = new
            {
                model = ttsModel,
                input = text,
                voice = voice
            };

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/audio/speech", requestBody);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream);
        }

        public async Task<WordResult> GetRandomWordAsync(string topic)
        {
            try
            {
                // Updated prompt to emphasize raw JSON
                var prompt = $@"Generate a single random English word related to the topic '{topic}'. Provide:
                - The word
                - Its meaning in English (a short definition, max 10 words)
                - Its phonetic transcription in IPA format
                Return the response as a raw JSON object, without markdown, code blocks, or any additional text. Example:
                {{
                    ""word"": ""example"",
                    ""meaning"": ""A sample or instance"",
                    ""phonetic"": ""/ɪɡˈzæmpəl/""
                }}";

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                new { role = "user", content = prompt }
            },
                    max_tokens = 100,
                    temperature = 0.7
                };

                // Set Accept header to enforce JSON response
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.PostAsJsonAsync("chat/completions", requestBody);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"OpenAI API Error: {response.StatusCode} - {errorContent}");
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw Response: {jsonResponse}");

                // Parse the response as JSON
                var jsonDoc = JsonDocument.Parse(jsonResponse);
                var content = jsonDoc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                if (string.IsNullOrWhiteSpace(content))
                {
                    Console.WriteLine("Error: Empty content received from API.");
                    return null;
                }

                content = content.Trim();
                Console.WriteLine($"Content Before Parsing: {content}");

                // Clean up markdown or stray characters if necessary
                if (content.StartsWith("```json") && content.EndsWith("```"))
                {
                    content = content.Substring(7, content.Length - 10).Trim();
                    Console.WriteLine($"Cleaned Content: {content}");
                }
                else if (!content.StartsWith("{") || !content.EndsWith("}"))
                {
                    var startIndex = content.IndexOf("{");
                    var endIndex = content.LastIndexOf("}");
                    if (startIndex >= 0 && endIndex > startIndex)
                    {
                        content = content.Substring(startIndex, endIndex - startIndex + 1);
                        Console.WriteLine($"Extracted JSON: {content}");
                    }
                    else
                    {
                        Console.WriteLine($"Error: Content is not valid JSON: {content}");
                        return null;
                    }
                }

                // Deserialize with options to handle relaxed JSON escaping
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                var wordResult = JsonSerializer.Deserialize<WordResult>(content, options);
                if (wordResult?.Word == null || wordResult.Meaning == null || wordResult.Phonetic == null)
                {
                    Console.WriteLine($"Error: Invalid JSON format from OpenAI response: {content}");
                    return null;
                }

                return wordResult;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Parsing Error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
                return null;
            }
        }
        // Generate IELTS listening exercise using ChatGPT API
        public async Task<IeltsListeningExercise> GenerateIeltsListeningExerciseAsync()
        {
            var prompt = @"Generate an IELTS listening practice exercise in English. Provide:
                - A short script (1000-1350 words, suitable for IELTS listening, e.g., a conversation or announcement).
                - 4 multiple-choice questions with 4 options each and the correct answer.
                Return the response as a raw JSON object, without markdown or additional text. Example:
                {
                    ""script"": ""A conversation between two people about a topic..."",
                    ""questions"": [
                        {
                            ""question"": ""What is the main topic?"",
                            ""options"": [""Option A"", ""Option B"", ""Option C"", ""Option D""],
                            ""answer"": ""Option A""
                        }
                    ]
                }";

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 500,
                temperature = 0.7
            };

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsJsonAsync("chat/completions", requestBody);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(jsonResponse);
            var content = jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException("Empty content received from OpenAI API");
            }

            content = content.Trim();
            if (content.StartsWith("```json") && content.EndsWith("```"))
            {
                content = content.Substring(7, content.Length - 10).Trim();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            var exercise = JsonSerializer.Deserialize<IeltsListeningExercise>(content, options);
            if (exercise == null || string.IsNullOrEmpty(exercise.Script) || exercise.Questions == null || !exercise.Questions.Any())
            {
                throw new JsonException($"Invalid IELTS listening exercise format: {content}");
            }

            return exercise;
        }


        // Generate audio from text
        public async Task<byte[]> GenerateSpeechAsync(string text)
        {
            var requestBody = new
            {
                model = "tts-1", // Mô hình text-to-speech của OpenAI
                input = text,    // Sử dụng "input" thay vì "text"
                voice = "alloy"  // Chọn một giọng hợp lệ, ví dụ "alloy"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/v1/audio/speech")
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }


    }
}
