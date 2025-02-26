using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Repositories.Interface
{
    public interface IRoleRepository
    {
        Role GetRoleByName(string name);
    }
}
