namespace DbAnalyzer.Core.Models.ReportModels.Interfaces
{
    public interface IReportItem
    {
        ReportItemStatus ReportItemStatus { get; set; }
        string Annotation { get; set; }
        string Query { get; set; }
    }

    public enum ReportItemStatus
    {
        None,
        Success,
        Warning,
        Error
    }
}
