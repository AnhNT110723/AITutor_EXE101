using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Services.Interface
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllRole();
        Task<Role> GetRoleByIdAsync(int roleId);

    }
}
