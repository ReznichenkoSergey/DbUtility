using System;
using System.Diagnostics.CodeAnalysis;

namespace DbAnalyzer.Core.Infrastructure.DbExplorers.DbIndexes.Models
{
    public class DbIndexBase : IEquatable<DbIndexBase>
    {
        public string IndexName { get; set; }
        public string TableName { get; set; }

        public bool Equals([AllowNull] DbIndexBase other)
            => IndexName.Equals(other.IndexName, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var other = obj as DbIndexBase;
            return IndexName.Equals(other.IndexName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
