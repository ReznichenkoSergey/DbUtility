using System;

namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes.Models
{
    public class DbIndex : DbIndexBase
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

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
