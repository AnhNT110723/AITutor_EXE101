using EXE_FAIEnglishTutor.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EXE_FAIEnglishTutor.Repositories.Interface

{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly FaiEnglishContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(FaiEnglishContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int? id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<T> GetByIdAsync(params object[] keyValues)
        {
            if (keyValues.Length == 2) // Nếu có 2 khóa chính
            {
                var key1 = keyValues[0];
                var key2 = keyValues[1];

                return await _dbSet.FirstOrDefaultAsync(e => EF.Property<int>(e, "CourseId") == (int)key1
                                                           && EF.Property<int>(e, "CategoryId") == (int)key2);
            }

            return await _dbSet.FindAsync(keyValues);
        }

    }
}
