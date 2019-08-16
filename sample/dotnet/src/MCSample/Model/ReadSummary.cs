using System.Collections.Generic;

namespace MCSample.Model
{
    public class ReadSummary
    {
        public IReadOnlyDictionary<string, IndexField> Fields { get; internal set; }

        public uint RowCount { get; internal set; }
    }
}

