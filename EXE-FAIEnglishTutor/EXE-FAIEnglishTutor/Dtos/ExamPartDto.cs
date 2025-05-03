namespace EXE_FAIEnglishTutor.Dtos
{
    public class ExamPartDto
    {
        public int PartId { get; set; }
        public string PartType { get; set; } = "";
        public string PartName { get; set; } = "";
        public int DefaultDuration { get; set; }
        public int QuestionCount { get; set; }
    }
}
