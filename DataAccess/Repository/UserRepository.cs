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
        public async Task<User?> FindById(int id) => await UserDAO.FindById(id);
        public async Task<User?> FindByEmail(string email) => await UserDAO.FindByEmail(email);
        public async Task Add(User user) => await UserDAO.Add(user);
        public async Task Update(User user) =>await UserDAO.Update(user);
    }
}
