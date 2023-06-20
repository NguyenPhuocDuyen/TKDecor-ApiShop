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
    public class UserAddressRepository : IUserAddressRepository
    {
        public async Task Add(UserAddress userAddress)
            => await UserAddressDAO.Add(userAddress);

        public async Task Delete(UserAddress userAddress)
            => await UserAddressDAO.Delete(userAddress);

        public async Task<UserAddress?> FindById(int id)
            => await UserAddressDAO.FindById(id);

        public async Task<List<UserAddress>> GetByUserId(int userId)
            => await UserAddressDAO.GetByUserId(userId);

        public async Task SetDefault(int userId, int? userAddressId)
            => await UserAddressDAO.SetDefault(userId, userAddressId);

        public async Task Update(UserAddress userAddress)
            => await UserAddressDAO.Update(userAddress);
    }
}
