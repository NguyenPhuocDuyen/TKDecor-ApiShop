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
        Task<bool> CheckLogin(string email, string password);
        Task<User> FindById(int id);
        Task<User> FindByEmail(string email);
        Task<User> Add(User user);
        Task<User> Update(User user);
    }
}
