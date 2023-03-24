using DbAnalyzer.Core.Infrastructure.DbExplorers.DbQueries.Interfaces;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbQueries.Models;
using DbAnalyzer.Core.Infrastructure.Reports.DbProcedures;
using DbAnalyzer.Core.Models.Parsers;
using DbAnalyzer.Core.Models.ReportModels;
using DbAnalyzer.Core.Models.ReportModels.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbAnalyzer.Core.Infrastructure.Reports.Queries
{
    public class ExpensiveQueriesReport : IExpensiveQueriesReport
    {
        private readonly IDbQueryExplorer _dbQueryExplorer;
        private readonly ILogger<ProceduresUsageReport> _logger;
        public List<ReportItem> ReportItems { get; private set; }

        public ExpensiveQueriesReport(IDbQueryExplorer dbQueryExplorer,
            ILogger<ProceduresUsageReport> logger)
        {
            _dbQueryExplorer = dbQueryExplorer;
            _logger = logger;
        }

        public async Task<IList<IReportItem>> GetReportAsync(ExpensiveQueryOrderingEnum ordering, int topAmount)
        {
            try
            {
                var result = new List<IReportItem>();
                var parser = new ExecPlanParser();
                var queries = await _dbQueryExplorer.GetExpensiveQueryListAsync(ordering, topAmount, true);
                
                Parallel.ForEach(queries, (query, token) =>
                {
                    var missingIndexes = parser.GetMissingIndexes(query.QueryPlan);
                    if (missingIndexes.Any())
                    {
                        foreach (var index in missingIndexes)
                        {
                            result.Add(new ReportItem()
                            {
                                ReportItemStatus = ReportItemStatus.Success,
                                Annotation = $"{query.GetAnnotation(index.Impact)}{Environment.NewLine}Query: {query.CompleteQueryText}",
                                Query = index.Query
                            });
                        }
                    }
                    var missingStatistics = parser.GetMissingStatistics(query.QueryPlan);
                    if (missingStatistics.Any())
                    {
                        foreach (var index in missingStatistics)
                        {
                            result.Add(new ReportItem()
                            {
                                ReportItemStatus = ReportItemStatus.Warning,
                                Annotation = "Missing statistics",
                                Query = index
                            });
                        }
                    }
                });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"NewIndexReport; Generate, Error= {ex.Message}");
                return null;
            }
        }
    }
}
