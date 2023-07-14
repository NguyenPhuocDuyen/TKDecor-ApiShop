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

        public async Task<UserAddress?> FindById(Guid id)
            => await UserAddressDAO.FindById(id);

        public async Task<List<UserAddress>> FindByUserId(Guid userId)
            => await UserAddressDAO.FindByUserId(userId);

        public Task<List<UserAddress>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task SetDefault(Guid userId, Guid? userAddressId = null)
        {
            try
            {
                var listAddressOfUser = await UserAddressDAO.FindByUserId(userId);
                foreach (var address in listAddressOfUser)
                {
                    address.IsDefault = false;
                    if (userAddressId != null)
                    {
                        if (userAddressId == address.UserAddressId)
                        {
                            address.IsDefault = true;
                        }
                    }
                    await UserAddressDAO.Update(address);
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task Update(UserAddress userAddress)
            => await UserAddressDAO.Update(userAddress);
    }
}
