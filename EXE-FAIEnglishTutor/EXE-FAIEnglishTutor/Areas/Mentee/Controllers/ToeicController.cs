using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

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
                var exams = await _toeicService.GetExamsByType(examTypeId);
                if (exams == null || !exams.Any())
                {
                    return Ok(new List<object>()); // Trả về mảng rỗng
                }
                return Ok(exams);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetExamByTypeAsync: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }


        [HttpGet("Mentee/Toeic/TestOnline/TestInput/{examTypeId}/{testName}")]
        public async Task<IActionResult> TestInput(int examTypeId, string testName)
        {
            if (examTypeId <= 0 || string.IsNullOrEmpty(testName))
            {
                return BadRequest("Invalid examTypeId or testName.");
            }



            // Tìm bài thi trong bảng Exam
            var exam = await _toeicService.GetExamsByTypeAndNameAsync(examTypeId, testName);

            if (exam == null)
            {
                return NotFound("Exam not found.");
            }


            // Tìm các part của bài thi
            var examParts = await _toeicService.GetExamPartsByTypeAndNameAsync(exam.ExamId);

            // Truyền dữ liệu vào View
            ViewData["TestId"] = exam.ExamId;
            ViewData["ExamTypeId"] = examTypeId;
            ViewData["ExamTitle"] = exam.Title;
            ViewData["Slug"] = exam.Slug;

            // Trả về view với dữ liệu exam
            return View("TestInput", examParts);
        }


        [HttpGet("Mentee/Toeic/TestOnline/StartTest/{examTypeId}/{slug}")]
        public async Task<IActionResult> TestToeic(int examTypeId, string slug)
        {
            try
            {
                // Tìm bài thi dựa trên examTypeId và slug
                var exam = await _toeicService.GetExamsByTypeAndNameAsync(examTypeId, slug);

                if (exam == null)
                {
                    return NotFound("Exam not found.");
                }

                // Lấy danh sách câu hỏi của bài thi
                var questions = await _toeicService.GetQuestionsForExamAsync(exam.ExamId);
                if (questions == null || !questions.Any())
                {
                    return NotFound("No questions found for this exam.");
                }

                // Truyền ExamId vào ViewData để sử dụng sau này
                ViewData["ExamId"] = exam.ExamId;
                ViewData["ExamTypeId"] = examTypeId;
                ViewData["Slug"] = slug;

                return View("TestToeic", questions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã xảy ra lỗi khi bắt đầu bài thi. Vui lòng thử lại sau.");
            }
        }
    }
}
