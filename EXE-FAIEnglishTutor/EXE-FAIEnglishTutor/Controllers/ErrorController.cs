using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HandleError(int statusCode)
        {
            ViewBag.StatusCode = statusCode;
            ViewBag.Message = statusCode switch
            {
                404 => "Page not found.",
                401 => "Unauthorized access.",
                403 => "Forbidden access.",
                402 => "Payment required.",
                408 => "Request Timeout.",
                500 => "Internal server error.",
                504 => "Gateway Timeout.",
                _ => "An error occurred."
            };
            return View("Error"); // View tên là Error.cshtml
        }
    }
}
