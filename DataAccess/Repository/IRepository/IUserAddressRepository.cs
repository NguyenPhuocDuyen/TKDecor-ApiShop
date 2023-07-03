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
        Task<List<UserAddress>> FindByUserId(long userId);
        Task<UserAddress?> FindById(long id);
        Task SetDefault(long userId, long? userAddressId);
        Task Add(UserAddress userAddress);
        Task Update(UserAddress userAddress);
    }
}
