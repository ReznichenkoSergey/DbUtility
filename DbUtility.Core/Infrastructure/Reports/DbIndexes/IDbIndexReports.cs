using DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes.Models;
using DbAnalyzer.Core.Models.ReportModels.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbAnalyzer.Core.Infrastructure.Reports.DbIndexes
{
    public interface IDbIndexReports
    {
        Task<IList<IReportItem>> GetUnusedIndexesReportAsync();
        Task<IList<IReportItem>> GetDublicateIndexesReportAsync();
        Task<IList<NonclusterIndexByProceduresUsageStatistic>> GetNonclusterIndexesUsageByProceduresStatisticsAsync();
        Task<IList<NonclusterIndexUsageStatistic>> GetNonclusterIndexesUsageStatisticsAsync();
    }
}
