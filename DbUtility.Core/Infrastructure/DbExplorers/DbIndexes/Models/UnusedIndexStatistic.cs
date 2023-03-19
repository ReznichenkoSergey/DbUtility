namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes.Models
{
    public class UnusedIndexStatistic
    {
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public int UserSeek { get; set; }
        public int UserScans { get; set; }
        public int UserLookups { get; set; }
        public int UserUpdates { get; set; }
        public int TableRows { get; set; }
        public int IndexSize { get; set; }
        public string GetAnnotation()
            => $"Name: {IndexName}, Table: {TableName}, Size: {IndexSize} Kb, TableRows: {TableRows}, UserScans: {UserScans}, UserSeek: {UserSeek}, UserUpdates: {UserUpdates}, UserLookups: {UserLookups}";
    }
}
