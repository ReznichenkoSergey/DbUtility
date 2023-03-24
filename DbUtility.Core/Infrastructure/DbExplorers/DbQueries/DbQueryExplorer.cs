using Dapper;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbQueries.Interfaces;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbQueries.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Resources = DbUtility.Core.Properties.Resources;

namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbQueries
{
    public class DbQueryExplorer : IDbQueryExplorer
    {
        readonly ILogger<DbQueryExplorer> _logger;
        private readonly IDbConnection _con;
        public DbQueryExplorer(ILogger<DbQueryExplorer> logger, IDbConnection con)
        {
            _logger = logger;
            _con = con;
        }

        /// <summary>
        /// Get index execution statistic
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ExpensiveQueryStatistics>> GetExpensiveQueryListAsync(ExpensiveQueryOrderingEnum ordering,  int topAmount = 50, bool includeExecutionPlan = false)
        {
            try
            {
                return await _con.QueryAsync<ExpensiveQueryStatistics>(string.Format(Resources.ExpensiveQueryPattern,
                    topAmount <= 0 ? 50 : topAmount, 
                    MapOrdering(ordering),
                    includeExecutionPlan ? ", qp.query_plan AS [QueryPlan]" : string.Empty,
                    includeExecutionPlan ? "CROSS APPLY sys.dm_exec_query_plan(plan_handle) AS qp" : string.Empty),
                    commandTimeout: 300);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ExpensiveQueryExplorer; GetStatisticsAsync, Error= {ex.Message}");
                return new List<ExpensiveQueryStatistics>();
            }
        }

        private string MapOrdering(ExpensiveQueryOrderingEnum ordering) => ordering switch
        {
            ExpensiveQueryOrderingEnum.LongRunningQuery => ExpensiveQueryOrderingPattern.LongRunningQuery,
            ExpensiveQueryOrderingEnum.FrequentlyRanQuery => ExpensiveQueryOrderingPattern.FrequentlyRanQuery,
            ExpensiveQueryOrderingEnum.HighDiskReadingQuery => ExpensiveQueryOrderingPattern.HighDiskReadingQuery,
            ExpensiveQueryOrderingEnum.HighCPUQuery => ExpensiveQueryOrderingPattern.HighCPUQuery,
            _ => throw new NotImplementedException(),
        };
        
    }
}
