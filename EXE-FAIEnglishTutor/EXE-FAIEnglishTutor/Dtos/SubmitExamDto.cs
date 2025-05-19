namespace EXE_FAIEnglishTutor.Dtos
{
    public class SubmitExamDto
    {
        public int ExamId { get; set; }
        public Dictionary<int, string> Answers { get; set; }
    }
}
