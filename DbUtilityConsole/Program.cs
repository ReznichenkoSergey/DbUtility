using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using DbUtilityConsole.Models;
using System.Threading.Tasks;
using DbAnalyzer.Core.Infrastructure.Reports.DbIndexes;
using DbAnalyzer.Core.Infrastructure.Reports.DbProcedures;
using DbAnalyzer.Core.Infrastructure.Reports.Queries;
using DbAnalyzer.Core.Infrastructure.DbExplorers.DbQueries.Models;
using System.Data;
using System.Data.SqlClient;
using DbAnalyzer.Core.Models.ReportModels.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DbUtilityConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            
            var serviceProvider = new ServiceCollection()
                    .AddCustomService()
                    .AddTransient<IDbConnection>((sp) => new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                    .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                    .AddLogging()
                    .BuildServiceProvider();

            var dbIndexReports = serviceProvider.GetRequiredService<IDbIndexReports>();
            var expensiveQueriesReport = serviceProvider.GetRequiredService<IExpensiveQueriesReport>();
            var proceduresUsageReport = serviceProvider.GetRequiredService<IProceduresUsageReport>();

            InitMessages();

            var fileName = string.Empty;            
            var key = Console.ReadLine();

            if(int.TryParse(key, out var index))
            {
                if (ReportInfo.ContainsKey(index))
                {
                    var value = (QueryTypeEnum)index;

                    GeneratingMessages();

                    var file = new FileCreator();
                    IList<IReportItem> result = null;
                    switch (value)
                    {
                        case QueryTypeEnum.ProceduresOptimization:
                            result = await proceduresUsageReport.GetReportAsync();
                            break;
                        case QueryTypeEnum.IndexesDublicatesOptimization:
                            result = await dbIndexReports.GetDublicateIndexesReportAsync();
                            break;
                        case QueryTypeEnum.IndexesUnusedOptimization:
                            result = await dbIndexReports.GetUnusedIndexesReportAsync();
                            break;
                        case QueryTypeEnum.ExpensiveQueriesOptimization:
                            result = await expensiveQueriesReport.GetReportAsync(ExpensiveQueryOrderingEnum.LongRunningQuery, 50);
                            break;
                        case QueryTypeEnum.IndexesUsageProceduresStatistic:
                            var statistics1 = await dbIndexReports.GetNonclusterIndexesUsageByProceduresStatisticsAsync();
                            fileName = file.SaveStatistics(Directory.GetCurrentDirectory(), statistics1);
                            break;
                        case QueryTypeEnum.IndexesUsageStatistic:
                            var statistics2 = await dbIndexReports.GetNonclusterIndexesUsageStatisticsAsync();
                            fileName = file.SaveStatistics(Directory.GetCurrentDirectory(), statistics2);
                            break;
                    }
                    if (result != null)
                    {
                        fileName = file.SaveReport(Directory.GetCurrentDirectory(), result);
                    }
                }
                else
                    Console.WriteLine("Not supported number!");
            }

            DoneMessages(fileName);
        }

        private static void InitMessages()
        {
            Console.WriteLine("Select report number to generate a report:");
            Console.WriteLine();
            foreach(var item in ReportInfo)
            {
                Console.WriteLine($"   {item.Key}. {item.Value}");
            }
            Console.WriteLine();
            Console.WriteLine("Selected number:");
            Console.WriteLine();
        }
        private static void GeneratingMessages()
        {
            Console.WriteLine();
            Console.WriteLine("Generating report ...");
            Console.WriteLine();
        }
        private static void DoneMessages(string fileName)
        {
            Console.WriteLine();
            if (!string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Generated file:");
                Console.WriteLine();
                Console.WriteLine(fileName);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"Nothing to save!");
                Console.WriteLine();
            }
            Console.WriteLine("Done!");
        }

        private static Dictionary<int, string> ReportInfo => new Dictionary<int, string>()
        {
            { (int)QueryTypeEnum.ProceduresOptimization, "Procedures Optimization" },
            { (int)QueryTypeEnum.IndexesDublicatesOptimization, "Indexes Dublicates Optimization" },
            { (int)QueryTypeEnum.IndexesUnusedOptimization, "Indexes Unused Optimization" },
            { (int)QueryTypeEnum.ExpensiveQueriesOptimization, "Expensive Queries Optimization" },
            { (int)QueryTypeEnum.IndexesUsageProceduresStatistic, "Indexes Usage Procedures Statistic" },
            { (int)QueryTypeEnum.IndexesUsageStatistic, "Indexes Usage Statistic" }
        };

        public enum QueryTypeEnum
        {
            ProceduresOptimization = 1,
            IndexesDublicatesOptimization,
            IndexesUnusedOptimization,
            ExpensiveQueriesOptimization,
            IndexesUsageProceduresStatistic,
            IndexesUsageStatistic
        }
    }
}
