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
                          TestId = t.ExamId // Ánh xạ ExamId thành TestId
                      }).ToList()
                  })
                  .ToListAsync();

            return exams;
        }
    }
}
