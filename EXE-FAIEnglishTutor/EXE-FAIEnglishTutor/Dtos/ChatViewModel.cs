using Microsoft.AspNetCore.Mvc.Rendering;

namespace EXE_FAIEnglishTutor.Dtos
{
    public class ChatViewModel
    {
        public List<SelectListItem> Situations { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "daily", Text = "Daily Conversation" },
            new SelectListItem { Value = "interview", Text = "Job Interview" },
            new SelectListItem { Value = "travel", Text = "Travel English" }
        };
    }
}
