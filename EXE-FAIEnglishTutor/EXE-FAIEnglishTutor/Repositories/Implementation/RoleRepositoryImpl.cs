using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace EXE_FAIEnglishTutor.Repositories.Implementation
{
    public class RoleRepositoryImpl : IRoleRepository
    {
        private readonly FaiEnglishContext _context;

        public RoleRepositoryImpl(FaiEnglishContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAllRole()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role> GetRoleByIdAsync(int roleId)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == roleId);
        }

        public Role GetRoleByName(string name)
        {
            return _context.Roles.FirstOrDefault(x => x.RoleName.Equals(name));
        }


    }
}
