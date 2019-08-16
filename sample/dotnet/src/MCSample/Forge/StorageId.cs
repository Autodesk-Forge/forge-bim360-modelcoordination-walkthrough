using System;

namespace MCSample.Forge
{
    public class StorageId
    {
        public string Bucket { get; set; }

        public string Key { get; set; }

        public static StorageId Parse(string ossUrn)
        {
            var parts = ossUrn.Split(new char[] { '/' }, 2);

            var resourceParts = parts[0].Split(new[] { ':' }, 4);

            return new StorageId
            {
                Bucket = resourceParts[3],
                Key = parts[1]
            };
        }
    }
}