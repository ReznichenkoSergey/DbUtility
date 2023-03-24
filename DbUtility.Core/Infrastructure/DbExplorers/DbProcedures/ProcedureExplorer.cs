using Dapper;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbProcedures.Interfaces;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbProcedures.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Resources = DbUtility.Core.Properties.Resources;

namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbProcedures
{
    public class ProcedureExplorer : IProcedureExplorer
    {
        readonly ILogger<IndexExplorer> _logger;
        private readonly string _connectionString;
        
        public ProcedureExplorer(ILogger<IndexExplorer> logger,
            IDbConnection con)
        {
            _logger = logger;
            _connectionString = con.ConnectionString;
        }

        public async Task<IEnumerable<string>> GetProcNamesFromDbAsync()
        {
            try
            {
                var con = new SqlConnection(_connectionString);
                return await con.QueryAsync<string>(Resources.GetProcedureNamesQuery);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ProcedureExplorer; GetProcedureNamesFromDbAsync, Error= {ex.Message}");
                return new List<string>();
            }
        }

        public IList<SqlParameter> GetParamsFromProcedure(string procName)
        {
            var list = new List<SqlParameter>();
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand()
                {
                    Connection = con,
                    CommandText = procName,
                    CommandType = CommandType.StoredProcedure
                };
                con.Open();

                SqlCommandBuilder.DeriveParameters(cmd);
                foreach (SqlParameter p in cmd.Parameters)
                {
                    list.Add(p);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ProcedureExplorer; GetParamsFromProcedure, Error= {ex.Message}");
            }
            return list;
        }

        public async Task<IEnumerable<ProcedureExecStatistic>> GetFullExecutionStatisticsAsync()
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                return await con.QueryAsync<ProcedureExecStatistic>(Resources.ProcStatExecutionCounterQuery, new { con.Database });
            }
            catch (Exception ex)
            {
                _logger.LogError($"ProcedureExplorer; GetFullExecutionStatisticsAsync, Error= {ex.Message}");
                return new List<ProcedureExecStatistic>();
            }
        }
    }
}
