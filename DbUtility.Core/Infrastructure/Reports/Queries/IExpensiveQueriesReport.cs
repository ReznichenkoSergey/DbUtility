using DbAnalyzer.Core.Infrastructure.DbExplorers.DbQueries.Models;
using DbAnalyzer.Core.Models.ReportModels.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbAnalyzer.Core.Infrastructure.Reports.Queries
{
    public interface IExpensiveQueriesReport
    {
        Task<IList<IReportItem>> GetReportAsync(ExpensiveQueryOrderingEnum ordering, int topAmount);
    }
}
