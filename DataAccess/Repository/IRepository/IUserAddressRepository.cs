using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IUserAddressRepository
    {
        Task<List<UserAddress>> GetByUserId(int userId);
        Task<UserAddress?> FindById(int id);
        Task SetDefault(int userId, int? userAddressId);
        Task Add(UserAddress userAddress);
        Task Update(UserAddress userAddress);
        Task Delete(UserAddress userAddress);
    }
}
