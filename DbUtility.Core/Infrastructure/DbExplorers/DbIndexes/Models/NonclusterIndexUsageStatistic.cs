using System.Collections.Generic;

namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes.Models
{
    public class NonclusterIndexUsageStatistic
    {
        public string IndexName { get; set; }
        public IList<string> ProcedureNames { get; set; }
    }
}
