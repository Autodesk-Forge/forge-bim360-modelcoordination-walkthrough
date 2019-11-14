using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Forge.Coordination
{
    public class IndexDocumentObjects
    {
        public string SeedFileVersionUrn { get; set; }

        public string DocumentVersionUrn { get; set; }

        public string IndexDocumentKey { get; set; }

        public string IndexFileKey { get; set; }

        public List<int> Objects { get; } = new List<int>();

        public void AddObjectId(int id)
        {
            if (!Objects.Contains(id))
            {
                Objects.Add(id);
            }
        }
    }
}
