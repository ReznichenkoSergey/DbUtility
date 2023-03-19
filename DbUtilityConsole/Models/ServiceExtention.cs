using DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes.Interfaces;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbProcedures.Interfaces;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbProcedures;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbQueries.Interfaces;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbQueries;
using DbAnalyzer.Core.Infrastructure.Reports.DbIndexes;
using DbAnalyzer.Core.Infrastructure.Reports.DbProcedures;
using DbAnalyzer.Core.Infrastructure.Reports.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace DbUtilityConsole.Models
{
    public static class ServiceExtention
    {
        public static IServiceCollection AddCustomService(this IServiceCollection services)
        {
            services
                .AddTransient<IIndexExplorer, IndexExplorer>()
                .AddTransient<IProcedureExplorer, ProcedureExplorer>()
                .AddTransient<IExecPlanExplorer, ExecPlanExplorer>()
                .AddTransient<IDbQueryExplorer, DbQueryExplorer>()
                .AddTransient<IProceduresUsageReport, ProceduresUsageReport>()
                .AddTransient<IExpensiveQueriesReport, ExpensiveQueriesReport>()
                .AddTransient<IDbIndexReports, DbIndexReports>();
            return services;
        }
    }
}
