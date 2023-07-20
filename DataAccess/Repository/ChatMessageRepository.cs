using BusinessObject;
using DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        public Task Add(ChatMessage entity)
        {
            throw new NotImplementedException();
        }

        public Task<ChatMessage?> FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ChatMessage>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task Update(ChatMessage entity)
        {
            throw new NotImplementedException();
        }
    }
}
