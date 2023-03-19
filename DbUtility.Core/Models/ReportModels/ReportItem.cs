using DbAnalyzer.Core.Models.ReportModels.Interfaces;
using System.Text.Json.Serialization;

namespace DbAnalyzer.Core.Models.ReportModels
{
    public class ReportItem : IReportItem
    {
        public ReportItemStatus ReportItemStatus { get; set; } = ReportItemStatus.None;

        public string Annotation { get; set; }

        public string Query { get; set; }
    }
}
