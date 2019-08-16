using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MCSample.Model
{
    public sealed class IndexResultJsonExporter
    {
        private readonly IReadOnlyDictionary<string, IndexField> _fields;
        private readonly FileInfo _resultFile;
        private readonly FileInfo _outputFile;

        public IndexResultJsonExporter(IReadOnlyDictionary<string, IndexField> fields, FileInfo resultFile, FileInfo outputFile)
        {
            _fields = fields ?? throw new System.ArgumentNullException(nameof(fields));
            _resultFile = resultFile ?? throw new System.ArgumentNullException(nameof(resultFile));
            _outputFile = outputFile ?? throw new System.ArgumentNullException(nameof(outputFile));
        }

        public async Task Export()
        {
            _resultFile.Refresh();

            if (!_resultFile.Exists)
            {
                throw new FileNotFoundException(_resultFile.FullName);
            }

            var reader = new IndexResultReader(_resultFile, _fields);

            using (var fout = _outputFile.Open(FileMode.Create))
            using (var sw = new StreamWriter(fout, Encoding.UTF8))
            {
                await reader.ReadToEndAsync(
                    async row =>
                    {
                        await sw.WriteLineAsync(JsonConvert.SerializeObject(row, Formatting.None));
                        return true;
                    },
                    true);
            }
        }
    }
}
