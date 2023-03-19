namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbQueries.Models
{
    public enum ExpensiveQueryOrderingEnum
    {
        FrequentlyRanQuery,
        HighDiskReadingQuery,
        HighCPUQuery,
        LongRunningQuery
    }
}
