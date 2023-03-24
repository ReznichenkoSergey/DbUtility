using Dapper;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes.Interfaces;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Resources = DbUtility.Core.Properties.Resources;

namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes
{
    public class IndexExplorer : IIndexExplorer
    {
        readonly IDbConnection _con;
        readonly ILogger<IndexExplorer> _logger;
        public IndexExplorer(ILogger<IndexExplorer> logger, IDbConnection con)
        {
            _logger = logger;
            _con = con;
        }

        /// <summary>
        /// Get index execution statistic
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<UnusedIndexStatistic>> GetUnusedIndexesAsync()
        {
            try
            {
                return await _con.QueryAsync<UnusedIndexStatistic>(Resources.UnusedIndexQuery);
            }
            catch (Exception ex)
            {
                _logger.LogError($"IndexExplorer; GetUnusedIndexesAsync, Error= {ex.Message}");
                return new List<UnusedIndexStatistic>();
            }
        }

        /// <summary>
        /// Get index dublicates 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DbIndex>> GetDublicateIndexesAsync()
        {
            try
            {
                return await _con.QueryAsync<DbIndex>(Resources.DublicateIndexesQuery);
            }
            catch (Exception ex)
            {
                _logger.LogError($"IndexExplorer; GetDublicateIndexesAsync, Error= {ex.Message}");
                return new List<DbIndex>();
            }
        }
    }
}
