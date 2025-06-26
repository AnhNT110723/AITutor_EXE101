using EXE_FAIEnglishTutor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using EXE_FAIEnglishTutor.Services.Interface.AI;
using EXE_FAIEnglishTutor.Dtos;
using System.Text;
using Newtonsoft.Json;
using EXE_FAIEnglishTutor.Common;
using Microsoft.CodeAnalysis.Scripting;
using System.Text.RegularExpressions;

namespace EXE_FAIEnglishTutor.Controllers.Lesson
{
    public class LessonController : Controller
    {
        private readonly FaiEnglishContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = Constants.BASE_URL; // Update with your actual API base URL

        public LessonController(FaiEnglishContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();

        }

        public IActionResult Index()
        {
            var lessons = _context.Situations.ToList();
            return View(lessons); // Gửi danh sách bài học ra view
        }

        public async Task<IActionResult> Step1(int id)
        {
            var lesson = await _context.Situations
                .FirstOrDefaultAsync(l => l.SituatuonId == id);

            if (lesson == null)
            {
                return NotFound();
            }

            try
            {
                // Call the API to get random words
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("topic", lesson.SituationName)
                });

                var response = await _httpClient.PostAsync($"http://localhost:5037/api/audio/generate-random-words", formData);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var words = System.Text.Json.JsonSerializer.Deserialize<List<WordResult>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                ViewBag.Lesson = lesson;
                ViewBag.Words = words;

                return View();
            }
            catch (HttpRequestException ex)
            {
                // Log the error and return a view with just the lesson
                // You might want to handle this differently based on your requirements
                ViewBag.Lesson = lesson;
                ViewBag.Words = new List<WordResult>();
                return View();
            }
        }
        

        private string[] regrex = { ". ", "! ", "? " };
        public async Task<IActionResult> Step2(int id)
        {
            var lesson = await _context.Situations

                .FirstOrDefaultAsync(l => l.SituatuonId == id);

            if (lesson == null)
            {
                return NotFound();
            }

            try
            {
                // Create API URL
                var apiUrl = $"{_apiBaseUrl}/api/audio/generate-ielts-listening";

                // Create request with raw string topic
                var jsonContent = $"\"{lesson.SituationName.Replace("\"", "\\\"")}\"";
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
                string script = Regex.Replace(exercise.Script, @"\bSpeaker\s*\d*\s*:\s*", ".", RegexOptions.IgnoreCase);
                ViewBag.Lesson = lesson;
                ViewBag.Script = script;
                ViewBag.Questions = exercise.Questions;
                ViewBag.Words = new List<WordResult>();
                ViewBag.AudioData = $"data:audio/mp3;base64,{audioBase64}";

                return View();
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Lesson = lesson;
                ViewBag.Script = $"Failed to connect to the API (Status: {ex.StatusCode}). Please try again.";
                ViewBag.Questions = new List<IeltsQuestion>();
                ViewBag.Words = new List<WordResult>();
                ViewBag.AudioData = string.Empty;
                return View();
            }
            catch (System.Text.Json.JsonException ex)
            {
                ViewBag.Lesson = lesson;
                ViewBag.Script = "Failed to process the API response. Please try again.";
                ViewBag.Questions = new List<IeltsQuestion>();
                ViewBag.Words = new List<WordResult>();
                ViewBag.AudioData = string.Empty;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Lesson = lesson;
                ViewBag.Script = $"An unexpected error occurred: {ex.Message}. Please try again.";
                ViewBag.Questions = new List<IeltsQuestion>();
                ViewBag.Words = new List<WordResult>();
                ViewBag.AudioData = string.Empty;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Step3(int id, string script, string audioData, string questions)
        {
            try
            {
                var lesson = await _context.Situations

                    .FirstOrDefaultAsync(l => l.SituatuonId == id);

                if (lesson == null)
                {
                    return NotFound("Lesson not found");
                }
                string scripts = Regex.Replace(script, @"\bSpeaker\s*\d*\s*:\s*", ".", RegexOptions.IgnoreCase);
                // Split script into sentences for line-by-line display
                var sentences = scripts.Split(regrex, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim().TrimEnd('.').TrimStart('.'))
                    .ToList();

                // Deserialize questions with null check
                var questionsList = !string.IsNullOrEmpty(questions)
                    ? JsonConvert.DeserializeObject<List<IeltsQuestion>>(questions)
                    : new List<IeltsQuestion>();

                ViewBag.Lesson = lesson;    
                ViewBag.Script = script;
                ViewBag.AudioData = audioData;
                ViewBag.Sentences = sentences;
                ViewBag.Questions = questionsList;

                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { Message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Step3(int id)
        {
            // Redirect to Step2 if accessed directly via GET
            return RedirectToAction("Step2", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Step2Pre(int id, string script, string audioData, string questions)
        {
            try
            {
                var lesson = await _context.Situations
                    .FirstOrDefaultAsync(l => l.SituatuonId == id);

                if (lesson == null)
                {
                    return NotFound("Lesson not found");
                }

                // Deserialize questions with null check
                var questionsList = !string.IsNullOrEmpty(questions)
                    ? JsonConvert.DeserializeObject<List<IeltsQuestion>>(questions)
                    : new List<IeltsQuestion>();

                // Create view model with the data from Step3
                ViewBag.Lesson = lesson;
                ViewBag.Script = script;
                ViewBag.AudioData = audioData;
                ViewBag.Questions = questionsList;
                ViewBag.Words = new List<WordResult>(); // Initialize empty list
                ViewBag.Sentences = script.Split(regrex, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim() + ".")
                    .ToList();

                return View("Step2");
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { Message = $"An error occurred: {ex.Message}" });
            }
        }
        

        public class DictationResult
        {
            public int SentenceNumber { get; set; }
            public string CorrectAnswer { get; set; }
            public string UserAnswer { get; set; }
            public bool IsCorrect { get; set; }
        }


       

        public class ErrorViewModel
        {
            public string Message { get; set; }
        }
    }
}
