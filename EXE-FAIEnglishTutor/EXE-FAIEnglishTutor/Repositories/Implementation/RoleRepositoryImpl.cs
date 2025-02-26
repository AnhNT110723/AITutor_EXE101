using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories.Interface;

namespace EXE_FAIEnglishTutor.Repositories.Implementation
{
    public class RoleRepositoryImpl : IRoleRepository
    {
        private readonly FaiEnglishContext _context;

        public RoleRepositoryImpl(FaiEnglishContext context)
        {
            _context = context;
        }
        public Role GetRoleByName(string name)
        {
            return _context.Roles.FirstOrDefault(x => x.RoleName.Equals(name));
        }
    }
}
