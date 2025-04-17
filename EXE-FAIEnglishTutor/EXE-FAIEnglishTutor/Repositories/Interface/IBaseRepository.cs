

namespace EXE_FAIEnglishTutor.Repositories.Interface
{
    // Interface định nghĩa các phương thức cơ bản
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int? id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<T> GetByIdAsync(params object[] keyValues);
    }
}
