using System.Threading.Tasks;

namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbProcedures.Interfaces
{
    public interface IExecPlanExplorer
    {
        Task<ExecPlanResultDto> GetExecPlanAsync(string procName);
    }

    public class ExecPlanResultDto
    {
        public ExecPlanResultType ExecPlanResultType { get; set; }
        public string Content { get; set; }
        public ExecPlanResultDto(ExecPlanResultType execPlanResultType, string content)
        {
            ExecPlanResultType = execPlanResultType;
            Content = content;
        }
    }

    public enum ExecPlanResultType
    {
        Success,
        Error
    }
}
