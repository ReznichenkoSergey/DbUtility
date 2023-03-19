using DbAnalyzer.Core.Models.ReportModels.Interfaces;

namespace DbAnalyzer.Core.Models.ReportModels
{
    public class Annotation : IComment
    {
        public string Text { get; set; }
    }
}
