using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Repositories.Interface.Mentee
{
    public interface IProfileRepository : IBaseRepository<User>
    {
        Task SaveChangeAysnc();
    }
}
