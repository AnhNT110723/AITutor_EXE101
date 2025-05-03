using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories.Interface;
using EXE_FAIEnglishTutor.Repositories.Interface.Mentee;
using Microsoft.EntityFrameworkCore;

namespace EXE_FAIEnglishTutor.Repositories.Implementation.Mentee
{
    public class ExamRepository : BaseRepository<Exam>, IExamRepositoy
    {
        public ExamRepository(FaiEnglishContext context) : base(context)
        {
        }

        public Task<Exam> CreateAsync(Exam exam)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ExamPartDto>> GetExamPartsByTypeAndNameAsync(int examId)
        {
            var examParts = await(from es in _context.ExamSections
                                  where es.ExamId == examId
                                  join ep in _context.ExamParts
                                  on es.PartId equals ep.PartId
                                  join q in _context.Questions
                                  on es.SectionId equals q.SectionId into questions
                                  select new ExamPartDto
                                  {
                                      PartId = ep.PartId,
                                      PartType = ep.PartType,
                                      PartName = ep.PartName,
                                      DefaultDuration = ep.DefaultDuration ?? 0,
                                      QuestionCount = questions.Count()
                                  })
                           .OrderBy(ep => ep.PartType)
                           .ThenBy(ep => ep.PartName)
                           .ToListAsync();

            return examParts;

        }

        public async Task<Exam> GetExamsByTypeAndNameAsync(int examTypeId, string testNameSlug)
        {
            var exam = await _context.Exams.Where(x => x.ExamTypeId == examTypeId && x.Slug == testNameSlug).FirstOrDefaultAsync();
            return exam;
        }

        public async Task<List<ExamDto>> GetExamsByTypeAsync(int typeId)
        {
            //var exams = await _context.Exams
            //    .Include(e => e.InverseParentExam) // lấy luôn con
            //    .Where(e => e.ParentExamId == null && e.ExamTypeId == typeId) // chỉ lấy root exam
            //    .ToListAsync();



            var exams = await _context.Exams
                  .Where(e => e.ExamTypeId == typeId && e.ParentExamId == null) // Chỉ lấy Exam cha
                  .Select(e => new ExamDto
                  {
                      Title = e.Title,
                      Tests = e.InverseParentExam.Select(t => new TestDto
                      {
                          Name = t.Title,
                          CompletedCount = _context.UserExamResults.Count(uer => uer.ExamId == t.ExamId),
                          TestId = t.ExamId, // Ánh xạ ExamId thành TestId
                          Slug = t.Slug
                      }).ToList()
                  })
                  .ToListAsync();

            return exams;
        }

        public async Task<List<QuestionDto>> GetQuestionsForExamAsync(int examId)
        {
                var questions = await(from es in _context.ExamSections
                                      where es.ExamId == examId
                                      join ep in _context.ExamParts
                                      on es.PartId equals ep.PartId
                                      join q in _context.Questions
                                      on es.SectionId equals q.SectionId
                                      select new QuestionDto
                                      {
                                          QuestionId = q.QuestionId,
                                          SectionId = q.SectionId,
                                          PartId = ep.PartId,
                                          PartType = ep.PartType,
                                          PartName = ep.PartName,
                                          QuestionText = q.QuestionText,
                                          AudioUrl = q.AudioUrl,
                                          ImageUrl = q.ImageUrl,
                                          QuestionType = q.QuestionType,
                                          AnswerA = q.Answers.FirstOrDefault(a => a.AnswerId % 4 == 1).AnswerText,
                                          AnswerB = q.Answers.FirstOrDefault(a => a.AnswerId % 4 == 2).AnswerText,
                                          AnswerC = q.Answers.FirstOrDefault(a => a.AnswerId % 4 == 3).AnswerText,
                                          AnswerD = q.Answers.FirstOrDefault(a => a.AnswerId % 4 == 0).AnswerText,
                                          CorrectAnswer = q.Answers.FirstOrDefault(a => a.IsCorrect ?? false).AnswerId % 4 == 1 ? "A" :
                                                          q.Answers.FirstOrDefault(a => a.IsCorrect ?? false).AnswerId % 4 == 2 ? "B" :
                                                          q.Answers.FirstOrDefault(a => a.IsCorrect ?? false).AnswerId % 4 == 3 ? "C" : "D"
                                      })
                          .OrderBy(q => q.PartType) // Listening trước, Reading sau
                          .ThenBy(q => q.PartName)  // Part 1, Part 2, ...
                          .ThenBy(q => q.QuestionId) // Sắp xếp theo thứ tự câu hỏi
                          .ToListAsync();

            return questions;
        }
    }
}
