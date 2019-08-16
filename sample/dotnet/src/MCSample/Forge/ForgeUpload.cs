using Newtonsoft.Json;
using System.IO;

namespace MCSample.Forge
{
    [JsonObject]
    public class ForgeUpload
    {
        private FileInfo _file;

        [JsonIgnore]
        public FileInfo File
        {
            get
            {
                return _file;
            }

            set
            {
                _file = value;

                Path = _file.FullName;
            }
        }

        [JsonProperty]
        public uint Version { get; set; }

        [JsonProperty]
        public string Path { get; set; }

        [JsonProperty]
        public ForgeEntity Storage { get; set; }

        [JsonProperty]
        public UploadResult Result { get; set; }
    }
}
