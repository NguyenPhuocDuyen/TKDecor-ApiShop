using BusinessObject;
using DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ChatRoomRepository : IChatRoomRepository
    {
        public Task Add(ChatRoom entity)
        {
            throw new NotImplementedException();
        }

        public Task<ChatRoom?> FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ChatRoom>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task Update(ChatRoom entity)
        {
            throw new NotImplementedException();
        }
    }
}
