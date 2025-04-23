using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Services.Interface.Mentee
{
    public interface IToeicTestService
    {
        Task<List<ExamDto>> GetExamsByType(int examType);
    }
}
