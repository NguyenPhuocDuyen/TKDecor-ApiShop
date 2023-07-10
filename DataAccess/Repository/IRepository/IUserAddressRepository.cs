﻿using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IUserAddressRepository : IRepository<UserAddress>
    {
        Task<List<UserAddress>> FindByUserId(Guid userId);
        Task SetDefault(Guid userId, Guid? userAddressId);
    }
}
