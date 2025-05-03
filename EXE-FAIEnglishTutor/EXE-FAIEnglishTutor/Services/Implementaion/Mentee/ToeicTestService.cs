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

        public async Task<List<ExamPartDto>> GetExamPartsByTypeAndNameAsync(int examId)
        {
            var examParts = await _examRepositoy.GetExamPartsByTypeAndNameAsync(examId);
            if (examParts == null || !examParts.Any())
            {
                throw new KeyNotFoundException($"No exam parts found for exam ID: {examId}");
            }
            return examParts;
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

        public async Task<Exam> GetExamsByTypeAndNameAsync(int examTypeId, string testNameSlug)
        {
            var exam = await _examRepositoy.GetExamsByTypeAndNameAsync(examTypeId, testNameSlug);
            if (exam == null)
            {
                throw new KeyNotFoundException($"No exam found for exam type ID: {examTypeId} and exam title: {testNameSlug}");
            }
            return exam;
        }

        public async Task<List<QuestionDto>> GetQuestionsForExamAsync(int examId)
        {
            var questions = await _examRepositoy.GetQuestionsForExamAsync(examId);
            if (questions == null)
            {
                throw new KeyNotFoundException($"No questions found for exam ID: {examId} ");
            }
            return questions;
        }
    }
}
