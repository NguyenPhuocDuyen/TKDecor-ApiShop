﻿using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ProductImageRepository : IProductImageRepository
    {
        public async Task Delete(ProductImage productImage)
            => await ProductImageDAO.Delete(productImage);
    }
}