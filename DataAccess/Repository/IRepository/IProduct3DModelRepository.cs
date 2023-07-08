using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IProduct3DModelRepository
    {
        Task<List<Product3DModel>> GetAll();
        Task<Product3DModel?> FindById(Guid id);
        Task Add(Product3DModel model);
        Task Update(Product3DModel model);
    }
}
