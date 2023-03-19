using DbAnalyzer.Core.Models.ReportModels.Interfaces;
using System;

namespace DbUtilityConsole.Models
{
    public static class Extentions
    {
        public static string GetReport(this IReportItem reportItem)
        {
            if (!string.IsNullOrEmpty(reportItem.Query))
                return $"/*{Environment.NewLine}{reportItem.Annotation}{Environment.NewLine}*/{Environment.NewLine}{reportItem.Query}{Environment.NewLine}";
            else
                return $"/*{Environment.NewLine}{reportItem.Annotation}{Environment.NewLine}*/{Environment.NewLine}";
        }
    }
}
