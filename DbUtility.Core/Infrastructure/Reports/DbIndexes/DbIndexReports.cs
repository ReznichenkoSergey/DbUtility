using DbAnalyzer.Core.Models.ReportModels.Interfaces;
using DbAnalyzer.Core.Models.ReportModels;
using Microsoft.Extensions.Logging;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes.Interfaces;
using DbAnalyzer.Core.Models.Helpers;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes.Models;
using DbAnalyzer.Core.Models.Parsers;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbProcedures.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace DbAnalyzer.Core.Infrastructure.Reports.DbIndexes
{
    public class DbIndexReports : IDbIndexReports
    {
        private readonly IIndexExplorer _indexExplorer;
        private readonly IProcedureExplorer _procedureExplorer;
        private readonly IExecPlanExplorer _execPlanExplorer;
        private readonly ILogger<DbIndexReports> _logger;
        public List<ReportItem> ReportItems { get; private set; }

        public DbIndexReports(IIndexExplorer indexExplorer,
            IProcedureExplorer procedureExplorer,
            IExecPlanExplorer execPlanExplorer,
            ILogger<DbIndexReports> logger)
        {
            _indexExplorer = indexExplorer;
            _procedureExplorer = procedureExplorer;
            _execPlanExplorer = execPlanExplorer;
            _logger = logger;
        }

        public async Task<IList<IReportItem>> GetUnusedIndexesReportAsync()
        {
            try
            {
                var spaceAmount = 0;
                var result = new List<IReportItem>();
                var items = await _indexExplorer.GetUnusedIndexesAsync();
                if (items != null && items.Any())
                {
                    var dbScriptGenerator = new DbScriptGenerator();
                    items
                        .ToList()
                        .ForEach(x =>
                        {
                            spaceAmount += x.IndexSize;
                            result.Add(new ReportItem()
                            {
                                ReportItemStatus = ReportItemStatus.Warning,
                                Annotation = x.GetAnnotation(),
                                Query = dbScriptGenerator.GetDropIndexScript(x.IndexName)
                            });
                        });
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetReportAsync (UnusedIndexesReport), Error= {ex.Message}");
                return null;
            }
        }

        public async Task<IList<NonclusterIndexByProceduresUsageStatistic>> GetNonclusterIndexesUsageByProceduresStatisticsAsync()
        {
            var list = new List<NonclusterIndexByProceduresUsageStatistic>();
            try
            {
                var parser = new ExecPlanParser();
                var procNames = await _procedureExplorer.GetProcNamesFromDbAsync();

                Parallel.ForEach(procNames, (procName, token) =>
                {
                    var planResult = _execPlanExplorer.GetExecPlanAsync(procName).GetAwaiter().GetResult();

                    if (!string.IsNullOrEmpty(planResult?.Content) && planResult.Content.Contains("IndexKind=\"NonClustered\"", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var indexes = parser.GetUsedNonclusterIndexes(planResult.Content);
                        if (indexes != null && indexes.Any())
                        {
                            list.Add(new NonclusterIndexByProceduresUsageStatistic()
                            {
                                ProcedureName = procName,
                                Indexes = indexes
                            });
                        }
                    }
                });
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetReportAsync; Generate, Error= {ex.Message}");
                return null;
            }
        }

        public async Task<IList<NonclusterIndexUsageStatistic>> GetNonclusterIndexesUsageStatisticsAsync()
        {
            try
            {
                var indexes = await GetNonclusterIndexesUsageByProceduresStatisticsAsync();
                var groups = indexes
                    .SelectMany(x => x.Indexes)
                    .Select(x => x.IndexName)
                    .Distinct()
                    .ToList();
                var result = new List<NonclusterIndexUsageStatistic>();
                foreach (var index in groups)
                {
                    var procedureName = indexes
                        .Where(x => x.Indexes.Any(y => y.IndexName.Equals(index)))
                        .Select(x => x.ProcedureName)
                        .Distinct()
                        .ToList();
                    result.Add(new NonclusterIndexUsageStatistic()
                    {
                        IndexName = index,
                        ProcedureNames = procedureName
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetNonclusterIndexesUsageStatisticsAsync; Generate, Error= {ex.Message}");
                return null;
            }
        }

        public async Task<IList<IReportItem>> GetDublicateIndexesReportAsync()
        {
            try
            {
                var result = new List<IReportItem>();

                var items = await _indexExplorer.GetDublicateIndexesAsync();
                if (items != null && items.Any())
                {
                    var indexInfo = await _indexExplorer.GetIndexesInfoAsync();
                    DbIndex temp = null;
                    var dbScriptGenerator = new DbScriptGenerator();
                    items
                        .OrderBy(x => x.TableName)
                        .ThenBy(x => x.KeyColumnList)
                        .ThenBy(x => x.IncludeColumnList)
                        .ToList()
                        .ForEach(x =>
                        {
                            if (temp != null && temp.Equals(x))
                            {
                                result.Add(new ReportItem()
                                {
                                    ReportItemStatus = ReportItemStatus.Warning,
                                    Query = dbScriptGenerator.GetDropIndexScript(x),
                                    Annotation = $"{GetSizeInfo(x.IndexName, indexInfo)}A copy of {temp.IndexName} index"
                                });
                            }
                            temp = x;
                        });
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetReportAsync (DublicateIndexes), Error= {ex.Message}");
                return null;
            }
        }

        private string GetSizeInfo(string indexName, IEnumerable<DbIndexInfo> indexInfo)
        {
            if(indexInfo == null)
                return string.Empty;

            var item = indexInfo.FirstOrDefault(y => y.IndexName.Equals(indexName));
            if (item == null)
                return string.Empty;

            return $"Size(Mb)= {item.IndexSize}, ";
        }
    }
}
