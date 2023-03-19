namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbProcedures.Models
{
    public class ProcedureExecStatistic
    {
        public string ProcedureName { get; set; }
        public string LastExecTime { get; set; }
        public string ModifyDate { get; set; }
        public string CreateDate { get; set; }
        public string ExecCount { get; set; }
    }
}
