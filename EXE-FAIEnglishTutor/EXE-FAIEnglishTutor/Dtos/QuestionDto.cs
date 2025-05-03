    namespace EXE_FAIEnglishTutor.Dtos
{
    public class QuestionDto
    {
        public int QuestionId { get; set; }
        public int? SectionId { get; set; }
        public int PartId { get; set; }
        public string PartType { get; set; }  // Listening hoặc Reading
        public string PartName { get; set; }  // Listening Part 1, Reading Part 5, ...
        public string QuestionText { get; set; }
        public string AudioUrl { get; set; }
        public string ImageUrl { get; set; }
        public string QuestionType { get; set; }
        public string AnswerA { get; set; }
        public string AnswerB { get; set; }
        public string AnswerC { get; set; }
        public string AnswerD { get; set; }
        public string CorrectAnswer { get; set; } // Chỉ dùng ở backend để tính điểm, không gửi lên frontend khi làm bài
    }
}
