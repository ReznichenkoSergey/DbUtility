using DbAnalyzer.Core.Models.ReportModels.Interfaces;

namespace DbAnalyzer.Core.Models.ReportModels
{
    public class Query : IComment
    {
        public string Text { get; set; }
    }
}
