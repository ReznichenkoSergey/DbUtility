namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbQueries.Models
{
    public static class ExpensiveQueryOrderingPattern
    {
        public static string FrequentlyRanQuery { get => "qs.execution_count DESC OPTION (RECOMPILE)"; }
        public static string HighDiskReadingQuery { get => "[TotalLogicalReads] DESC OPTION (RECOMPILE)"; }
        public static string HighCPUQuery { get => "[AvgWorkerTime] DESC OPTION (RECOMPILE)"; }
        public static string LongRunningQuery { get => "[AvgElapsedTime] DESC OPTION (RECOMPILE)"; }
    }
}
