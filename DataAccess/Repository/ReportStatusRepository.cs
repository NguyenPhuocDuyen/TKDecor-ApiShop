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
    public class ReportStatusRepository : IReportStatusRepository
    {
        public async Task<List<ReportStatus>> GetAll()
            => await ReportStatusDAO.GetAll();
    }
}
