using BE_TKDecor.Service.IService;
using BusinessObject;

namespace BE_TKDecor.Service
{
    public class UserService : IUserService
    {
        private readonly TkdecorContext _context;

        public UserService(TkdecorContext context)
        {
            _context = context;
        }

        public async Task<User?> GetById(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }
    }
}
