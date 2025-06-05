using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Services.Implementaion.AI;
using EXE_FAIEnglishTutor.Services.Interface.AI;
using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostcardController : ControllerBase
    {
        private readonly SpeechService _speechService;

        public PostcardController(SpeechService speechService)
        {   
            _speechService = speechService;
        }
       
        [HttpPost("generate-postcard")]
        public async Task<IActionResult> GeneratePostcard()
        {
            try
            {
                var prompt = "Summarize and synthesize the 10 most recent Vietnamese articles into a postcard 10000 words.";
                var postcard = await _speechService.GeneratePostcardAsync(prompt);

                if (string.IsNullOrWhiteSpace(postcard))
                {
                    return BadRequest("Không nhận được nội dung từ API.");
                }

                return Ok(new { postcard });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Lỗi khi tạo postcard.", detail = ex.Message });
            }
        }
    }
}
