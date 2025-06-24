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
                    max_tokens = 4000, // khoảng tối đa model hỗ trợ cho 3.5-turbo

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

        public async Task<List<WordResult>> GetRandomWordsAsync(string topic)
        {
            try
            {
                // Prompt yêu cầu 10 từ tiếng Anh liên quan đến topic
                var prompt = $@"Generate exactly 10 random English words related to the topic '{topic}'. For each word, provide:
        - The word
        - Its meaning in English (a short definition, max 10 words)
        - Its phonetic transcription in IPA format
        Return the response as a raw JSON array of objects, without markdown, code blocks, or any additional text. Example:
        [
            {{
                ""word"": ""example"",
                ""meaning"": ""A sample or instance"",
                ""phonetic"": ""/ɪɡˈzæmpəl/""
            }},
            {{
                ""word"": ""test"",
                ""meaning"": ""An examination or trial"",
                ""phonetic"": ""/tɛst/""
            }}
        ]";

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                new { role = "user", content = prompt }
            },
                    max_tokens = 500, // Tăng max_tokens để chứa 10 từ
                    temperature = 0.7
                };

                // Set Accept header để đảm bảo phản hồi là JSON
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

                // Parse phản hồi JSON
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

                // Loại bỏ markdown hoặc ký tự thừa nếu có
                if (content.StartsWith("```json") && content.EndsWith("```"))
                {
                    content = content.Substring(7, content.Length - 10).Trim();
                    Console.WriteLine($"Cleaned Content: {content}");
                }
                else if (!content.StartsWith("[") || !content.EndsWith("]"))
                {
                    var startIndex = content.IndexOf("[");
                    var endIndex = content.LastIndexOf("]");
                    if (startIndex >= 0 && endIndex > startIndex)
                    {
                        content = content.Substring(startIndex, endIndex - startIndex + 1);
                        Console.WriteLine($"Extracted JSON: {content}");
                    }
                    else
                    {
                        Console.WriteLine($"Error: Content is not valid JSON array: {content}");
                        return null;
                    }
                }

                // Deserialize với tùy chọn hỗ trợ JSON linh hoạt
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                var wordResults = JsonSerializer.Deserialize<List<WordResult>>(content, options);
                if (wordResults == null || wordResults.Count == 0 || wordResults.Any(w => w?.Word == null || w.Meaning == null || w.Phonetic == null))
                {
                    Console.WriteLine($"Error: Invalid JSON format from OpenAI response: {content}");
                    return null;
                }

                return wordResults;
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
        public async Task<WordResult> GetRandomWordAsync(string topic)
        {
            try
            {
                // Prompt yêu cầu 1 từ tiếng Anh liên quan đến topic
                var prompt = $@"Generate exactly one random English word related to the topic '{topic}'. Provide:
- The word
- Its meaning in English (a short definition, max 10 words)
- Its phonetic transcription in IPA format
Return the response as a raw JSON array of objects, without markdown, code blocks, or any additional text. Example:
[
    {{
        ""word"": ""example"",
        ""meaning"": ""A sample or instance"",
        ""phonetic"": ""/ɪɡˈzæmpəl/""
    }}
]";

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                new { role = "user", content = prompt }
            },
                    max_tokens = 100, // Giảm max_tokens cho 1 từ
                    temperature = 0.7
                };

                // Set Accept header để đảm bảo phản hồi là JSON
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

                // Parse phản hồi JSON
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

                // Loại bỏ ký tự thừa nếu có
                if (!content.StartsWith("[") || !content.EndsWith("]"))
                {
                    var startIndex = content.IndexOf("[");
                    var endIndex = content.LastIndexOf("]");
                    if (startIndex >= 0 && endIndex > startIndex)
                    {
                        content = content.Substring(startIndex, endIndex - startIndex + 1);
                        Console.WriteLine($"Extracted JSON: {content}");
                    }
                    else
                    {
                        Console.WriteLine($"Error: Content is not valid JSON array: {content}");
                        return null;
                    }
                }

                // Deserialize với tùy chọn hỗ trợ JSON linh hoạt
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                var wordResults = JsonSerializer.Deserialize<List<WordResult>>(content, options);
                if (wordResults == null || wordResults.Count == 0 || wordResults[0].Word == null || wordResults[0].Meaning == null || wordResults[0].Phonetic == null)
                {
                    Console.WriteLine($"Error: Invalid JSON format from OpenAI response: {content}");
                    return null;
                }

                return wordResults[0];
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
        public async Task<IeltsListeningExercise> GenerateIeltsListeningExerciseAsync(string topic)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new Exception("Missing OpenAI API key. Please set your API key.");

            const string endpoint = "https://api.openai.com/v1/chat/completions";
            var prompt = $@"Generate an IELTS listening practice exercise in English on the topic: '{topic.Replace("\"", "\\\"")}'.
- Create a conversation script (150-180 words, 2-3 speakers, natural IELTS style, various question types, no fewer than 135 or more than 195 words).
- Write exactly 14 multiple-choice questions, each with 4 options, one correct answer. Start from easy (main idea, details) to harder (inference, intent).
- Return a raw JSON object (no markdown/code block/extra text). All strings must be correctly JSON-escaped.
Format:
{{
  ""script"": ""..."",
  ""questions"": [
    {{""type"":""multiple-choice"",""question"":""..."",""options"":[""..."",""..."",""..."",""...""] ,""answer"":""...""}}
  ]
}}";

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 1800, // Đủ cho script + 14 câu hỏi
                temperature = 0.6
            };

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(50));
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            int maxRetries = 3;
            IeltsListeningExercise exercise = null;
            string lastError = "";
            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync(endpoint, requestBody, cts.Token);
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        // Nếu lỗi key/quota, không retry
                        if (jsonResponse.Contains("API key") || jsonResponse.Contains("quota") || jsonResponse.Contains("unauthorized"))
                            throw new Exception($"OpenAI API error: {response.StatusCode}\n{jsonResponse}");
                        lastError = $"API error: {response.StatusCode}\n{jsonResponse}";
                        continue;
                    }

                    // Lấy content
                    using var jsonDoc = JsonDocument.Parse(jsonResponse);
                    var content = jsonDoc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();
                    if (string.IsNullOrWhiteSpace(content))
                        throw new InvalidOperationException("Empty content received from OpenAI API");

                    // Loại bỏ markdown/code block
                    content = content.Trim();
                    if (content.StartsWith("```json")) content = content[7..];
                    if (content.EndsWith("```")) content = content[..^3];
                    content = content.Trim(new char[] { '\r', '\n', '`' });

                    // Deserialize
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        AllowTrailingCommas = true,
                        ReadCommentHandling = JsonCommentHandling.Skip
                    };
                    exercise = JsonSerializer.Deserialize<IeltsListeningExercise>(content, options);

                    // Validate
                    if (exercise == null || string.IsNullOrWhiteSpace(exercise.Script) || exercise.Questions == null || exercise.Questions.Count() != 14)
                        throw new JsonException("Invalid exercise format or missing questions.\n" + content);

                    // Đếm số từ bằng regex cho chính xác
                    var wordCount = System.Text.RegularExpressions.Regex.Matches(exercise.Script, @"\b\w+\b").Count;
                    if (wordCount < 135 || wordCount > 195)
                    {
                        lastError = $"Script word count ({wordCount}) is not within 135-195 range.";
                        continue; // Không hợp lệ, thử lại
                    }

                    // Thành công!
                    return exercise;
                }
                catch (JsonException ex)
                {
                    lastError = "JSON parse error: " + ex.Message;
                    continue;
                }
                catch (TaskCanceledException)
                {
                    lastError = "Request timed out.";
                    continue;
                }
                catch (Exception ex)
                {
                    lastError = ex.Message;
                    break; // Lỗi nghiêm trọng, không retry nữa
                }
            }

            throw new InvalidOperationException($"Failed to generate IELTS exercise after {maxRetries} attempts. Last error: {lastError}");
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
