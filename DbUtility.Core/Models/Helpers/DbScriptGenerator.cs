using DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes.Models;
using DbAnalyzer.Core.Models.ExecPlanModels;
using SqlAnalyzer.Core.Models.ExecPlanModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbAnalyzer.Core.Models.Helpers
{
    public class DbScriptGenerator
    {
        public string GetCreateIndexScript(MissingIndexesMissingIndexGroupMissingIndex index)
        {
            var bld = new StringBuilder();
            bld.AppendFormat("CREATE NONCLUSTERED INDEX [IX_{1}_{2}]{0}",
            Environment.NewLine,//0
                        index.Table.Replace("[", string.Empty).Replace("]", string.Empty),//1
                        string.Join("_", index.ColumnGroup.Where(x => x.Usage.Equals("EQUALITY") || x.Usage.Equals("INEQUALITY")).SelectMany(x => x.Column).Select(x => x.Name.Replace("[", string.Empty).Replace("]", string.Empty))));//2
            bld.AppendFormat("ON {1}.{2} ({3}){0}",
                Environment.NewLine,//0
                index.Schema,//1
                index.Table,//2
                string.Join(",", index.ColumnGroup.Where(x => x.Usage.Equals("EQUALITY") || x.Usage.Equals("INEQUALITY")).SelectMany(x => x.Column).Select(x => x.Name)));

            var include = index.ColumnGroup.Where(x => x.Usage.Equals("INCLUDE")).SelectMany(x => x.Column).Select(x => x.Name);
            if (include.Any())
            {
                bld.AppendFormat("INCLUDE ({1}){0}",
                    Environment.NewLine,//0
                    string.Join(",", index.ColumnGroup.Where(x => x.Usage.Equals("INCLUDE")).SelectMany(x => x.Column).Select(x => x.Name)));//2
            }
            return bld.ToString();
        }

        public string GetDropIndexScript(DbIndex dbIndex) => GetDropIndexScript(dbIndex.IndexName);

        public string GetDropIndexScript(string indexName) => $"DROP INDEX {indexName}";

        public IEnumerable<string> GetCreateStatisticsScript(ColumnsWithNoStatistics columnsWithNoStatistics)
            => columnsWithNoStatistics
            .Items
            .Select(x => $"CREATE STATISTICS [{x.Column}Statistic_{DateTime.Now:MMddyyyy}] ON {x.Schema}.{x.Table} ({x.Column})");

    }
}
