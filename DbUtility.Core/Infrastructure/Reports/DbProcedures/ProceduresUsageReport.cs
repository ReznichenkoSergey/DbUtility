using DbAnalyzer.Core.Infrastructure.DbExplorers.DbProcedures.Interfaces;
using DbAnalyzer.Core.Models.Parsers;
using DbAnalyzer.Core.Models.ReportModels;
using DbAnalyzer.Core.Models.ReportModels.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbAnalyzer.Core.Infrastructure.Reports.DbProcedures
{
    public class ProceduresUsageReport : IProceduresUsageReport
    {
        private readonly IProcedureExplorer _procedureExplorer;
        private readonly IExecPlanExplorer _execPlanExplorer;
        private readonly ILogger<ProceduresUsageReport> _logger;
        public List<ReportItem> ReportItems { get; private set; }

        public ProceduresUsageReport(IProcedureExplorer procedureExplorer,
            IExecPlanExplorer execPlanExplorer,
            ILogger<ProceduresUsageReport> logger)
        {
            _procedureExplorer = procedureExplorer;
            _execPlanExplorer = execPlanExplorer;
            _logger = logger;
        }

        public async Task<IList<IReportItem>> GetReportAsync()
        {
            try
            {
                var result = new List<IReportItem>();
                var parser = new ExecPlanParser();
                var procNames = await _procedureExplorer.GetProcNamesFromDbAsync();
                var procExecStatistics = await _procedureExplorer.GetFullExecutionStatisticsAsync();

                Parallel.ForEach(procNames, (procName, token) =>
                {
                    var planResult = _execPlanExplorer.GetExecPlanAsync(procName).GetAwaiter().GetResult();

                    if (!string.IsNullOrEmpty(planResult?.Content) && planResult.Content.Contains("IndexKind=\"NonClustered\"", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var list = parser.GetUsedNonclusterIndexes(planResult.Content);

                    }

                    if (planResult.ExecPlanResultType == ExecPlanResultType.Success && !string.IsNullOrEmpty(planResult.Content))
                    {
                        var missingIndexes = parser.GetMissingIndexes(planResult.Content);
                        var procStat = procExecStatistics.FirstOrDefault(x => x.ProcedureName.Equals(procName));
                        var stat = "not found";
                        if (procStat != null)
                        {
                            stat = $"ExecCount= {procStat.ExecCount}, LastExecTime= {procStat.LastExecTime}, CreateDate= {procStat.CreateDate}";
                        }
                        if (missingIndexes.Any())
                        {
                            foreach (var index in missingIndexes)
                            {
                                result.Add(new ReportItem()
                                {
                                    ReportItemStatus = ReportItemStatus.Success,
                                    Annotation = $"Procedure {procName}, impact: {index.Impact}%, statistics: {stat}",
                                    Query = index.Query
                                });
                            }
                        }
                        else
                        {
                            result.Add(new ReportItem()
                            {
                                ReportItemStatus = ReportItemStatus.None,
                                Annotation = $"Procedure {procName}, no indexes need, statistics: {stat}"
                            });
                        }
                        //
                        var missingStatistics = parser.GetMissingStatistics(planResult.Content);
                        if (missingStatistics.Any())
                        {
                            foreach (var index in missingStatistics)
                            {
                                result.Add(new ReportItem()
                                {
                                    ReportItemStatus = ReportItemStatus.Warning,
                                    Annotation = "Missing statistics",
                                    Query = index
                                });
                            }
                        }
                    }
                    else
                    {
                        result.Add(new ReportItem()
                        {
                            ReportItemStatus = ReportItemStatus.Error,
                            Annotation = $"Procedure {procName}, execution plan generation error: {planResult.Content}"
                        });
                    }
                });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"NewIndexReport; Generate, Error= {ex.Message}");
                return null;
            }
        }
    }
}
