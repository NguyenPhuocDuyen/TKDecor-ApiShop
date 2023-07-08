using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();
        Task<User?> FindById(Guid id);
        Task<User?> FindByEmail(string email);
        Task Add(User user);
        Task Update(User user);
    }
}
