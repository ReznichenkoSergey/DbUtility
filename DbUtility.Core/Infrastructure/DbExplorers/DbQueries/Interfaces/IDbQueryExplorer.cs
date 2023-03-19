using DbAnalyzer.Core.Infrastructure.DbExplorers.DbQueries.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbQueries.Interfaces
{
    public interface IDbQueryExplorer
    {
        Task<IEnumerable<ExpensiveQueryStatistics>> GetExpensiveQueryListAsync(ExpensiveQueryOrderingEnum ordering, int topAmount, bool includeExecutionPlan);
    }
}
