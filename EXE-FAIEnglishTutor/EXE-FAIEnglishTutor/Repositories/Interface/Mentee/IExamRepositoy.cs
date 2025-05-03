using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Repositories.Interface.Mentee
{
    public interface IExamRepositoy : IBaseRepository<Exam>
    {
        Task<Exam> CreateAsync(Exam exam);
        Task<List<ExamDto>> GetExamsByTypeAsync(int type);
        Task<Exam> GetExamsByTypeAndNameAsync(int examTypeId, string testNameSlug);
        Task<List<ExamPartDto>> GetExamPartsByTypeAndNameAsync(int examId);
        Task<List<QuestionDto>> GetQuestionsForExamAsync(int examId);
    }
}
