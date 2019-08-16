using System;

namespace MCQuery.Command
{
    public class LastQueryState
    {
        public Guid Container { get; set; }

        public Guid ModelSet { get; set; }

        public uint Verison { get; set; }

        public string Query { get; set; }

        public long Size { get; set; }

        public bool Success { get; set; }

        public string ResultPath { get; set; }
    }
}
