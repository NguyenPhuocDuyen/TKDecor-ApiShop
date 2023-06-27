using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    internal class ReportStatusDAO
    {
        internal static async Task<List<ReportStatus>> GetAll()
        {
            try
            {
                using var context = new TkdecorContext();
                var reportStatus = await context.ReportStatuses
                    .ToListAsync();
                return reportStatus;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
