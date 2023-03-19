using DbAnalyzer.Core.Models.Helpers;
using SqlAnalyzer.Core.Models.ExecPlanModels;
using System;
using System.Text;
using static DbAnalyzer.Core.Models.Parsers.ExecPlanParser;

namespace SqlAnalyzer.Core.Models
{
    public static class Extentions
    {
        public static MissingIndex GetMissingIndex(this MissingIndexes missingIndexes, IFormatProvider formatProvider)
        {
            var result = new MissingIndex();
            var scriptGenerator = new DbScriptGenerator();

            var bld = new StringBuilder();
            foreach (var group in missingIndexes.Items)
            {
                result.Impact = decimal.Parse(group.Impact, formatProvider);
                foreach (var index in group.MissingIndex)
                {
                    bld.AppendLine(scriptGenerator.GetCreateIndexScript(index));
                }
            }
            result.Query = bld.ToString();
            return result;
        }
    }
}
