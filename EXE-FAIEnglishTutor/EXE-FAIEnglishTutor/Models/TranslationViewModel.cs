namespace EXE_FAIEnglishTutor.Models
{
    public class TranslationViewModel
    {
        public string Language { get; set; } 
        public Dictionary<string, string> TranslatedContent { get; set; }
    }
}
