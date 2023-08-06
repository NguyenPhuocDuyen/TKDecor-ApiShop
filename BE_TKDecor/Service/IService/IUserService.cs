using BusinessObject;

namespace BE_TKDecor.Service.IService
{
    public interface IUserService
    {
        Task<User?> GetById(Guid id); 
    }
}
