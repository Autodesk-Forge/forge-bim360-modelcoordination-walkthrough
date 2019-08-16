using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MCSample
{
    public static class SampleFileManager
    {
        public static class Sample
        {
            public static class Audubon
            {
                public const string V1 = "audubon-v1";

                public const string V2 = "audubon-v2";
            }
        }

        private readonly static Lazy<DirectoryInfo> _lazySampleFolder = new Lazy<DirectoryInfo>(() =>
            new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "..", "..", "..", "files")));

        private readonly static Lazy<DirectoryInfo> _lazyStateCacheDir = new Lazy<DirectoryInfo>(() =>
        {
            var dir = new DirectoryInfo(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nucleus", "state")));

            if (!dir.Exists)
            {
                dir.Create();
            }

            return dir;
        });

        public static DirectoryInfo SampleDirectory => _lazySampleFolder.Value;

        public static DirectoryInfo StateDirectory => _lazyStateCacheDir.Value;

        public static FileInfo GetFile(string sampleName, string fileName)
            => new FileInfo(Path.Combine(_lazySampleFolder.Value.FullName, $"{sampleName}", $"{fileName}"));

        public static void ResetStateCache()
        {
            _lazyStateCacheDir.Value.Delete(true);

            _lazyStateCacheDir.Value.Create();

            _lazyStateCacheDir.Value.Refresh();
        }

        public static async Task SaveState<T>(T state) where T : class, new()
        {
            if (state != null)
            {
                var file = new FileInfo(Path.Combine(_lazyStateCacheDir.Value.FullName, typeof(T).Name));

                file.Refresh();

                using (var fout = file.Open(FileMode.Create))
                using (var sw = new StreamWriter(fout, Encoding.UTF8))
                {
                    await sw.WriteAsync(JsonConvert.SerializeObject(state, Formatting.Indented));
                }
            }
        }

        public static async Task<T> LoadSavedState<T>() where T : class, new()
        {
            var state = default(T);

            var file = new FileInfo(Path.Combine(_lazyStateCacheDir.Value.FullName, typeof(T).Name));

            if (file.Exists)
            {
                using (var fout = file.OpenRead())
                using (var sw = new StreamReader(fout, Encoding.UTF8))
                {
                    state = JsonConvert.DeserializeObject<T>(await sw.ReadToEndAsync());
                }
            }

            return state;
        }

        public static FileInfo NewStatePath(string fileName) => new FileInfo(Path.Combine(_lazyStateCacheDir.Value.FullName, fileName));
    }
}
