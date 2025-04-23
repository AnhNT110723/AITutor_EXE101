using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Areas.Mentee.Controllers
{
    [Area("Mentee")]
    public class ToeicController : Controller
    {
        private readonly IToeicTestService _toeicService;
        public ToeicController(IToeicTestService toeicTestService) 
        {
            _toeicService = toeicTestService;
        }

        [HttpGet("Mentee/Toeic")]
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet("Mentee/Toeic/TestOnline")]
        public IActionResult TestOnline()
        {
            return View("TestOnline");
        }

        [HttpGet("Mentee/Toeic/TestOnline/{examTypeId}")]
        public async Task<IActionResult> GetExamByTypeAsync(int examTypeId = 1)
        {
            try
            {
                Console.WriteLine($"Processing GetExamByTypeAsync with examTypeId: {examTypeId}");
                var exams = await _toeicService.GetExamsByType(examTypeId);
                if (exams == null || !exams.Any())
                {
                    Console.WriteLine($"No exams found for examTypeId: {examTypeId}");
                    return Ok(new List<object>()); // Trả về mảng rỗng
                }
                Console.WriteLine($"Exams retrieved: {System.Text.Json.JsonSerializer.Serialize(exams)}");
                return Ok(exams);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetExamByTypeAsync: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }


        [HttpGet("Mentee/Toeic/TestOnline/TestInput")]
        public IActionResult TestInput()
        {
            return View("TestInput");
        }


        [HttpGet("Mentee/Toeic/TestOnline/TestInput/TestToeic")]
        public IActionResult TestToeic()
        {
            return View("TestToeic");
        }
    }
}
