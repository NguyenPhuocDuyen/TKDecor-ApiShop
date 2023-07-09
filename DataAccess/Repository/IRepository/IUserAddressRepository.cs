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
        Task<List<UserAddress>> FindByUserId(Guid userId);
        Task<UserAddress?> FindById(Guid id);
        Task SetDefault(Guid userId, Guid? userAddressId);
        Task Add(UserAddress userAddress);
        Task Update(UserAddress userAddress);
    }
}
