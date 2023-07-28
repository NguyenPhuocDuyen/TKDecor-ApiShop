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
    public class Product3DModelRepository : IProduct3DModelRepository
    {
        public async Task Add(Product3DModel model)
            => await Product3DModelDAO.Add(model);

        public async Task<Product3DModel?> FindById(Guid id)
            => await Product3DModelDAO.FindById(id);

        public async Task<List<Product3DModel>> GetAll()
            => await Product3DModelDAO.GetAll();

        public async Task Update(Product3DModel model)
            => await Product3DModelDAO.Update(model);
    }
}
