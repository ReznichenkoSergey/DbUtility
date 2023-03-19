using Dapper;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbQueries.Interfaces;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbQueries.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbQueries
{
    public class DbQueryExplorer : IDbQueryExplorer
    {
        readonly ILogger<DbQueryExplorer> _logger;
        private readonly IDbConnection _con;
        private readonly string ExpensiveQueryPattern = @"SELECT TOP({0}) qs.execution_count [ExecutionCount],
                                                            (qs.total_logical_reads)*8/1024.0 [TotalLogicalReads],
                                                            (qs.total_logical_reads/qs.execution_count)*8/1024.0 AS [AvgLogicalReads],
                                                            (qs.total_worker_time)/1000.0 [TotalWorkerTime],
                                                            (qs.total_worker_time/qs.execution_count)/1000.0 [AvgWorkerTime],
                                                            (qs.total_elapsed_time)/1000.0 [TotalElapsedTime],
                                                            (qs.total_elapsed_time/qs.execution_count)/1000.0 AS [AvgElapsedTime],
                                                            qs.creation_time AS [CreationTime],
                                                            t.text AS [CompleteQueryText] 
                                                            {2}
                                                        FROM sys.dm_exec_query_stats qs WITH (NOLOCK)
                                                        CROSS APPLY sys.dm_exec_sql_text(plan_handle) AS t
                                                        {3}
                                                        WHERE t.dbid = DB_ID()
                                                        ORDER BY {1}";


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
                return await _con.QueryAsync<ExpensiveQueryStatistics>(string.Format(ExpensiveQueryPattern,
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
