using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories.Interface.Mentee;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using Microsoft.Data.SqlClient;

namespace EXE_FAIEnglishTutor.Services.Implementaion.Mentee
{
    public class ToeicTestService : IToeicTestService
    {
        private readonly IExamRepositoy _examRepositoy;
        public ToeicTestService(IExamRepositoy examRepositoy) {
            _examRepositoy = examRepositoy;
        }
        public async Task<List<ExamDto>> GetExamsByType(int examTypeId)
        {

                var exams = await _examRepositoy.GetExamsByTypeAsync(examTypeId);
                if(exams == null || !exams.Any())
                {
                    throw new KeyNotFoundException($"No exams found for exam type ID: {examTypeId}");
                }
                return exams;

        }
    }
}
