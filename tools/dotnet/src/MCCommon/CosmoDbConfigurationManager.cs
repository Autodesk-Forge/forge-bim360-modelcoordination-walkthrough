using Newtonsoft.Json;
using System;
using System.Composition;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MCCommon
{
    [Export(typeof(ICosmoDbConfigurationManager))]
    internal sealed class CosmoDbConfigurationManager : ICosmoDbConfigurationManager
    {
        private readonly Lazy<FileInfo> _lazyConfig;

        private readonly Lazy<DirectoryInfo> _lazyConfigDir = new Lazy<DirectoryInfo>(() =>
        {
            var dir = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nucleus"));

            if (!dir.Exists)
            {
                dir.Create();
            }

            return dir;
        });

        public CosmoDbConfigurationManager()
        {
            _lazyConfig = new Lazy<FileInfo>(() => new FileInfo(Path.Combine(_lazyConfigDir.Value.FullName, "cosmo")));
        }

        public async Task<CosmoDbConfiguration> Get()
        {
            CosmoDbConfiguration config = null;

            _lazyConfig.Value.Refresh();

            if (_lazyConfig.Value.Exists)
            {
                using (var fs = _lazyConfig.Value.OpenRead())
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    config = JsonConvert.DeserializeObject<CosmoDbConfiguration>(await sr.ReadToEndAsync());
                }
            }

            return config;
        }

        public async Task Set(CosmoDbConfiguration configuration)
        {
            using (var fs = _lazyConfig.Value.Open(FileMode.Create))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
            {
                await sw.WriteAsync(JsonConvert.SerializeObject(configuration, Formatting.Indented));
            }
        }
    }
}
