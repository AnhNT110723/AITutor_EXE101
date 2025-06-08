using EXE_FAIEnglishTutor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using EXE_FAIEnglishTutor.Services.Interface.AI;
using EXE_FAIEnglishTutor.Dtos;
using System.Text;
using Newtonsoft.Json;

namespace EXE_FAIEnglishTutor.Controllers.Lesson
{
    public class LessonController : Controller
    {
        public LessonViewModel viewModel1 { get; set; }
        private readonly FaiEnglishContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "http://localhost:5037"; // Update with your actual API base URL

        public LessonController(FaiEnglishContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();

        }

        public IActionResult Index()
        {
            var lessons = _context.Lessons.ToList();
            return View(lessons); // Gửi danh sách bài học ra view
        }

        public async Task<IActionResult> Step1(int id)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.LessonId == id);

            if (lesson == null)
            {
                return NotFound();
            }

            try
            {
                // Call the API to get random words
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("topic", lesson.Title)
                });

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/audio/generate-random-words", formData);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var words = System.Text.Json.JsonSerializer.Deserialize<List<WordResult>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var viewModel = new LessonViewModel
                {
                    Lesson = lesson,
                    Words = words
                };

                return View(viewModel);
            }
            catch (HttpRequestException ex)
            {
                // Log the error and return a view with just the lesson
                // You might want to handle this differently based on your requirements
                return View(new LessonViewModel { Lesson = lesson, Words = new List<WordResult>() });
            }
        }
        public async Task<IActionResult> Step2(int id)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.LessonId == id);

            if (lesson == null)
            {
                return NotFound();
            }

            try
            {
                // Create API URL
                var apiUrl = $"{_apiBaseUrl}/api/audio/generate-ielts-listening";

                // Create request with raw string topic
                var jsonContent = $"\"{lesson.Title.Replace("\"", "\\\"")}\"";
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Make API call
                var response = await _httpClient.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                // Validate JSON before deserialization
                JsonDocument.Parse(responseContent); // Throws JsonException if invalid JSON

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                var exercise = System.Text.Json.JsonSerializer.Deserialize<IeltsListeningExercise>(responseContent, options);

                if (exercise == null || string.IsNullOrEmpty(exercise.Script) || exercise.Questions == null || exercise.Questions.Count != 14)
                {
                    throw new System.Text.Json.JsonException($"Invalid response from IELTS listening API. Expected 14 questions, got {exercise?.Questions?.Count ?? 0}");
                }

                // Generate audio for the script
                var formData = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("text", exercise.Script)
        });

                var audioResponse = await _httpClient.PostAsync($"{_apiBaseUrl}/api/audio/generate-english", formData);
                audioResponse.EnsureSuccessStatusCode();

                // Get audio bytes and convert to Base64
                var audioBytes = await audioResponse.Content.ReadAsByteArrayAsync();
                var audioBase64 = Convert.ToBase64String(audioBytes);

                var viewModel = new LessonViewModel
                {
                    Lesson = lesson,
                    Script = exercise.Script,
                    Questions = exercise.Questions.Take(2).ToList(),
                    Words = new List<WordResult>(),
                    AudioData = $"data:audio/mp3;base64,{audioBase64}" // Store as data URL
                };


                
                return View(viewModel);
            }
            catch (HttpRequestException ex)
            {
                return View(new LessonViewModel
                {
                    Lesson = lesson,
                    Script = $"Failed to connect to the API (Status: {ex.StatusCode}). Please try again.",
                    Questions = new List<IeltsQuestion>(),
                    Words = new List<WordResult>(),
                    AudioData = string.Empty
                });
            }
            catch (System.Text.Json.JsonException ex)
            {
                return View(new LessonViewModel
                {
                    Lesson = lesson,
                    Script = "Failed to process the API response. Please try again.",
                    Questions = new List<IeltsQuestion>(),
                    Words = new List<WordResult>(),
                    AudioData = string.Empty
                });
            }
            catch (Exception ex)
            {
                return View(new LessonViewModel
                {
                    Lesson = lesson,
                    Script = $"An unexpected error occurred: {ex.Message}. Please try again.",
                    Questions = new List<IeltsQuestion>(),
                    Words = new List<WordResult>(),
                    AudioData = string.Empty
                });
            }
        }
        
    }
    public class LessonViewModel
    {
        public Models.Lesson Lesson { get; set; }
        public List<WordResult> Words { get; set; }
        public List<IeltsQuestion> Questions { get; set; }
        public string Script { get; set; }
        public string AudioData { get; set; } // Base64 encoded audio data
        public List<string> Sentences { get; set; }
    }
    
    public class Step2ViewModel { }
}
