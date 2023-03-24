using DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes.Interfaces
{
    public interface IIndexExplorer
    {
        Task<IEnumerable<UnusedIndexStatistic>> GetUnusedIndexesAsync();
        Task<IEnumerable<DbIndex>> GetDublicateIndexesAsync();
        Task<IEnumerable<DbIndexInfo>> GetIndexesInfoAsync();
    }
}
