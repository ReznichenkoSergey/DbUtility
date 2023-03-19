using System.Collections.Generic;

namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes.Models
{
    public class NonclusterIndexByProceduresUsageStatistic
    {
        public string ProcedureName { get; set; }
        public IList<DbIndexBase> Indexes { get; set; }
    }
}
