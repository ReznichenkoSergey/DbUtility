using System;
using System.Diagnostics.CodeAnalysis;

namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes.Models
{
    public class DbIndex : DbIndexBase, IEquatable<DbIndex>
    {
        public string KeyColumnList { get; set; }
        public string IncludeColumnList { get; set; }
        public override bool Equals(object obj)
        {
            var temp = (DbIndex)obj;
            return TableName.Equals(temp.TableName)
                && KeyColumnList.Equals(temp.KeyColumnList)
                && IncludeColumnList.Equals(temp.IncludeColumnList);
        }

        public bool Equals([AllowNull] DbIndex other)
        {
            if(other == null) 
                return false;

            return TableName.Equals(other.TableName)
                && KeyColumnList.Equals(other.KeyColumnList)
                && IncludeColumnList.Equals(other.IncludeColumnList);
        }

        public override int GetHashCode()
            => IndexName.GetHashCode();
    }

    public class DbIndexInfo: DbIndexBase
    {
        /// <summary>
        /// (Mb)
        /// </summary>
        public decimal IndexSize { get; set; }
        public DateTime? StatisticLastUpdated { get; set; }
        public decimal AvgFragmentationInPercent { get; set; }
    }
}
