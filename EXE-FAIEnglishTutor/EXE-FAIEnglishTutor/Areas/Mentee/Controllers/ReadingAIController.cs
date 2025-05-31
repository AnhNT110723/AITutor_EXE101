using EXE_FAIEnglishTutor.Services.Interface.AI;
using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Areas.Mentee.Controllers
{
    [Area("Mentee")]
    public class ReadingAIController : Controller
    {
        private readonly IReadingAIService _readingService;

        public ReadingAIController(IReadingAIService readingAIService)
        {
            _readingService = readingAIService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GeneratePassage(string level, string levelScore)
        {
            var passage = await _readingService.GenerateReadingPassageAsync(level, levelScore);
            ViewBag.Passage = passage;
            ViewBag.Level = level;
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ExplainVocabulary(string word, string context)
        {
            var explanation = await _readingService.ExplainVocabularyAsync(word, context);
            ViewBag.VocabularyExplanation = explanation;
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ExplainGrammar(string sentence)
        {
            var explanation = await _readingService.ExplainGrammarAsync(sentence);
            ViewBag.GrammarExplanation = explanation;
            return View("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Summarize(string passage)
        {
            var summary = await _readingService.SummarizePassageAsync(passage);
            ViewBag.Summary = summary;
            return View("Index");
        }   
    }
}
