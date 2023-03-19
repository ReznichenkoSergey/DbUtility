using System.Collections.Generic;

namespace DbAnalyzer.Core.Models.ReportModels.Interfaces
{
    public interface IReport
    {
        public IList<IReportItem> ReportItems { get; set; }
        public IList<string> Result { get; set; }
    }
}
