using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class UserRepository : IUserRepository
    {
        public async Task<List<User>> GetAll() => await UserDAO.GetAll();
        public async Task<bool> CheckLogin(string email, string password) => await UserDAO.CheckLogin(email, password);
        public async Task<User> FindById(int id) => await UserDAO.FindById(id);
        public async Task<User> FindByEmail(string email) => await UserDAO.FindByEmail(email);
        public async Task<User> Add(User user) => await UserDAO.Add(user);
        public async Task<User> Update(User user) =>await UserDAO.Update(user);
    }
}
