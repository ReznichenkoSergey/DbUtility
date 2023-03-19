using DbAnalyzer.Core.Models.ReportModels.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbAnalyzer.Core.Infrastructure.Reports.DbProcedures
{
    public interface IProceduresUsageReport
    {
        Task<IList<IReportItem>> GetReportAsync();
    }
}
