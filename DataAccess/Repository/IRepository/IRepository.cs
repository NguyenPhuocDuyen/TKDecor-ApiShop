using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IRepository<T>
    {
        Task<List<T>> GetAll();
        Task<T?> FindById(Guid id);
        Task Add(T entity);
        Task Update(T entity);
    }
}
