using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Services.Interface.Mentee
{
    public interface IToeicTestService
    {
        Task<List<ExamDto>> GetExamsByType(int examType);
        Task<Exam> GetExamsByTypeAndNameAsync(int examTypeId, string testNameSlug);
        Task<List<ExamPartDto>> GetExamPartsByTypeAndNameAsync(int examId);
        Task<List<QuestionDto>> GetQuestionsForExamAsync(int examId);
    }
}
