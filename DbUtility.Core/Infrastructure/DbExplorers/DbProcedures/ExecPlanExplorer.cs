using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbProcedures.Interfaces;
using Microsoft.Extensions.Logging;

namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbProcedures
{
    public class ExecPlanExplorer : IExecPlanExplorer
    {
        readonly ILogger<ExecPlanExplorer> _logger;
        private readonly IProcedureExplorer _explorer;
        private readonly string _connectionString;
        private const string EnableExPlan = "SET SHOWPLAN_XML ON;";
        private const string DisableExPlan = "SET SHOWPLAN_XML OFF;";

        public ExecPlanExplorer(ILogger<ExecPlanExplorer> logger, 
            IProcedureExplorer explorer,
            IDbConnection con)
        {
            _logger = logger;
            _explorer = explorer;
            _connectionString = con.ConnectionString;
        }

        public async Task<ExecPlanResultDto> GetExecPlanAsync(string procName)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var con = new SqlConnection(_connectionString);
                    var query = GetPreparedQuery(procName);

                    using var cmd = new SqlCommand()
                    {
                        Connection = con,
                        CommandText = EnableExPlan,
                        CommandType = CommandType.Text
                    };
                    con.Open();

                    //ON
                    cmd.ExecuteNonQuery();

                    //Result
                    cmd.CommandText = query;
                    var planContent = cmd.ExecuteScalar();
                    var planContentText = planContent != null ? planContent.ToString() : string.Empty;

                    //OFF
                    cmd.CommandText = DisableExPlan;
                    cmd.ExecuteNonQuery();

                    return new ExecPlanResultDto(ExecPlanResultType.Success, planContentText);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"ExecPlanExplorer; GetExecPlanAsync, Error= {ex.Message}");
                    return new ExecPlanResultDto(ExecPlanResultType.Error, ex.Message);
                }
            });
        }

        private string GetPreparedQuery(string procName)
        {
            var procParams = _explorer.GetParamsFromProcedure(procName);
            var pars = new List<string>();
            var bld = new StringBuilder();
            foreach (SqlParameter p in procParams)
            {
                bld.AppendFormat("DECLARE {0} {1}{2}",
                        p.ParameterName,
                        p.SqlDbType,
                        p.Size > 0 ? $" ({p.Size})" : string.Empty
                        );
                bld.AppendLine();
                pars.Add(string.Format("{0}{1}",
                    p.ParameterName,
                    p.Direction == ParameterDirection.Output ? " OUTPUT" : string.Empty));
            }
            bld.AppendLine();
            bld.AppendFormat($"EXECUTE @RETURN_VALUE = {procName} {string.Join(",", pars.Where(x => !x.Equals("@RETURN_VALUE")))}");
            return bld.ToString();
        }
    }
}
