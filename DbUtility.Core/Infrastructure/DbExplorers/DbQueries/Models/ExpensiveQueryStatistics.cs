using DbAnalyzer.Core.Models;
using System;
using System.Text.Json.Serialization;

namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbQueries.Models
{
    public class ExpensiveQueryStatistics
    {
        /// <summary>
        /// Execution Count
        /// </summary>
        public long ExecutionCount { get; set; }
        /// <summary>
        /// Total Logical Reads (MB)
        /// </summary>
        public double TotalLogicalReads { get; set; }
        /// <summary>
        /// Avg Logical Reads (MB)
        /// </summary>
        public double AvgLogicalReads { get; set; }
        /// <summary>
        /// Total Worker Time (ms)
        /// </summary>
        public double TotalWorkerTime { get; set; }
        /// <summary>
        /// Avg Worker Time (ms)
        /// </summary>
        public double AvgWorkerTime { get; set; }
        /// <summary>
        /// Total Elapsed Time (ms)
        /// </summary>
        public double TotalElapsedTime { get; set; }
        /// <summary>
        /// Avg Elapsed Time (ms)
        /// </summary>
        public double AvgElapsedTime { get; set; }
        /// <summary>
        /// Creation Time
        /// </summary>
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// Complete Query Text
        /// </summary>
        public string CompleteQueryText { get; set; }
        /// <summary>
        /// Query Plan
        /// </summary>
        public string QueryPlan { get; set; }

        public DbPbjectTypeEnum GetDbPbjectType
        {
            get
            {
                if (string.IsNullOrEmpty(CompleteQueryText))
                {
                    return DbPbjectTypeEnum.None;
                }
                if (CompleteQueryText.Contains("create procedure"))
                {
                    return DbPbjectTypeEnum.Procedure;
                }
                else if (CompleteQueryText.Contains("create function"))
                {
                    return DbPbjectTypeEnum.Function;
                }
                else if (CompleteQueryText.Contains("select") && CompleteQueryText.Contains("from"))
                {
                    return DbPbjectTypeEnum.Query;
                }
                return DbPbjectTypeEnum.None;
            }
        }

    }

}
