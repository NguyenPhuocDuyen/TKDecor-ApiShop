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
    public class ProductReviewInteractionStatusRepository : IProductReviewInteractionStatusRepository
    {
        public async Task<List<ProductReviewInteractionStatus>> GetAll()
            => await ProductReviewInteractionStatusDAO.GetAll();
    }
}
