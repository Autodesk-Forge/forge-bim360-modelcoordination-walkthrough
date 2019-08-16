using Newtonsoft.Json;
using System;
using System.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCCommon
{
    [Shared]
    [Export(typeof(IForgeAppConfigurationManager))]
    internal class LocalForgeAppConfigurationManager : IForgeAppConfigurationManager
    {
        private readonly Lazy<FileInfo> _lazyStg;
        private readonly Lazy<FileInfo> _lazyProd;
        private readonly Lazy<FileInfo> _lazyDev;
        private readonly Lazy<FileInfo> _lazyToken;

        private readonly Lazy<DirectoryInfo> _lazyConfigDir = new Lazy<DirectoryInfo>(() =>
        {
            var dir = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nucleus"));

            if (!dir.Exists)
            {
                dir.Create();
            }

            return dir;
        });

        public DirectoryInfo ConfigDirectory => _lazyConfigDir.Value;

        public LocalForgeAppConfigurationManager()
        {
            _lazyStg = new Lazy<FileInfo>(() => new FileInfo(Path.Combine(_lazyConfigDir.Value.FullName, "stg")));
            _lazyProd = new Lazy<FileInfo>(() => new FileInfo(Path.Combine(_lazyConfigDir.Value.FullName, "prod")));
            _lazyDev = new Lazy<FileInfo>(() => new FileInfo(Path.Combine(_lazyConfigDir.Value.FullName, "dev")));
            _lazyToken = new Lazy<FileInfo>(() => new FileInfo(Path.Combine(_lazyConfigDir.Value.FullName, "token")));
        }

        public async Task<ForgeAppConfiguration> GetEnvironmentConfiguration(ForgeEnvironment environment)
        {
            switch (environment)
            {
                case ForgeEnvironment.Development:
                    return await LoadSavedEnvironment(ForgeEnvironment.Development);

                case ForgeEnvironment.Staging:
                    return await LoadSavedEnvironment(ForgeEnvironment.Staging);

                case ForgeEnvironment.Production:
                    return await LoadSavedEnvironment(ForgeEnvironment.Production);

                default:
                    throw new NotSupportedException();
            }
        }

        public async Task SaveEnvironmentConfiguration(ForgeApp configuration)
        {
            var outFile = GetPathForEnvironment(configuration.Environment);

            using (var fs = outFile.Open(FileMode.Create))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
            {
                await sw.WriteAsync(JsonConvert.SerializeObject(configuration, Formatting.Indented));
            }
        }

        public async Task<string> GetCachedToken()
        {
            string token = null;

            _lazyToken.Value.Refresh();

            if (_lazyToken.Value.Exists)
            {
                using (var fs = _lazyToken.Value.OpenRead())
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    token = await sr.ReadToEndAsync();
                }
            }

            return token;
        }

        public async Task CacheToken(string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                using (var fs = _lazyToken.Value.Open(FileMode.Create))
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    await sw.WriteAsync(token);
                }
            }
        }

        public Task DeleteCachedToken()
        {
            return Task.Run(() =>
            {
                _lazyToken.Value.Refresh();

                if (_lazyToken.Value.Exists)
                {
                    _lazyToken.Value.Delete();
                }
            });
        }

        public void Reset()
        {
            _lazyConfigDir.Value.Refresh();

            if (_lazyConfigDir.Value.Exists)
            {
                _lazyConfigDir.Value.Delete(recursive: true);
            }

            _lazyConfigDir.Value.Create();
        }

        public async Task<ForgeAppConfiguration[]> GetCurrentConfiguraitons()
        {
            var apps = new ForgeAppConfiguration[3];

            apps[0] = await LoadSavedEnvironment(ForgeEnvironment.Development);
            apps[1] = await LoadSavedEnvironment(ForgeEnvironment.Staging);
            apps[2] = await LoadSavedEnvironment(ForgeEnvironment.Production);

            return apps;
        }

        public async Task SetDefaultEnvironment(ForgeEnvironment environment)
        {
            var environments = await this.GetCurrentConfiguraitons();

            foreach (var env in environments)
            {
                if (env.Environment == environment)
                {
                    env.IsDefault = true;
                }
                else
                {
                    env.IsDefault = false;
                }

                await SaveEnvironmentConfiguration(env);
            }
        }

        public async Task<ForgeAppConfiguration> GetDefaultConfiguration() => (await GetCurrentConfiguraitons()).SingleOrDefault(c => c.IsDefault);

        private async Task<ForgeAppConfiguration> LoadSavedEnvironment(ForgeEnvironment environment)
        {
            var file = GetPathForEnvironment(environment);

            file.Refresh();

            ForgeAppConfiguration config = null;

            if (file.Exists)
            {
                using (var fs = file.OpenRead())
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    config = JsonConvert.DeserializeObject<ForgeAppConfiguration>(await sr.ReadToEndAsync());
                }
            }
            else
            {
                config = new ForgeAppConfiguration();
            }

            switch (environment)
            {
                case ForgeEnvironment.Development:
                    {
                        config.Environment = ForgeEnvironment.Development;
                        config.Host = "developer-stg.api.autodesk.com";
                    }
                    break;

                case ForgeEnvironment.Staging:
                    {
                        config.Environment = ForgeEnvironment.Staging;
                        config.Host = "developer-stg.api.autodesk.com";
                    }
                    break;

                case ForgeEnvironment.Production:
                    {
                        config.Environment = ForgeEnvironment.Production;
                        config.Host = "developer.api.autodesk.com";
                    }
                    break;

                default:
                    throw new NotSupportedException();
            }

            return config;
        }

        private FileInfo GetPathForEnvironment(ForgeEnvironment environment)
        {
            switch (environment)
            {
                case ForgeEnvironment.Development:
                    return _lazyDev.Value;

                case ForgeEnvironment.Staging:
                    return _lazyStg.Value;

                case ForgeEnvironment.Production:
                    return _lazyProd.Value;

                default:
                    throw new InvalidOperationException("Environemnt not supported");
            }
        }
    }
}
