using DbAnalyzer.Core.Models.ReportModels.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DbUtilityConsole.Models
{
    public class FileCreator
    {
        public string SaveReport(string folderName, IList<IReportItem> reportItems)
        {
            if (reportItems == null || !reportItems.Any() || string.IsNullOrEmpty(folderName))
            {
                return string.Empty;
            }

            var fileName = GetFileName(folderName, FileType.Sql);
            File.WriteAllLines(fileName, reportItems.Select(x => x.GetReport()));

            return fileName;
        }

        public string SaveStatistics<T>(string folderName, IList<T> reportItems)
        {
            if (reportItems == null || !reportItems.Any() || string.IsNullOrEmpty(folderName))
            {
                return string.Empty;
            }

            var content = JsonSerializer.Serialize(reportItems, new JsonSerializerOptions()
            {
                WriteIndented = true
            });

            var fileName = GetFileName(folderName, FileType.Json);
            File.WriteAllText(fileName, content);

            return fileName;
        }

        private string GetFileName(string folderName, FileType fileType)
        {
            folderName = fileType == FileType.Sql
                ? $"{folderName}\\Reports"
                : $"{folderName}\\Statistics";

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            return fileType == FileType.Sql 
                ? $"{folderName}//Report_{DateTime.Now:yyyyMMdd}_{Guid.NewGuid()}.sql"
                : $"{folderName}//Statistic_{DateTime.Now:yyyyMMdd}_{Guid.NewGuid()}.json";
        }

        private enum FileType
        {
            Sql,
            Json
        }
        
    }
}
