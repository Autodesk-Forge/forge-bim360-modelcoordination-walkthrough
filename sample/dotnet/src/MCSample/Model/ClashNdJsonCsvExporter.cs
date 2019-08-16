using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSample.Model
{
    public class ClashNdJsonCsvExporter
    {
        private readonly FileInfo _outputFile;

        public ClashNdJsonCsvExporter(FileInfo outputFile)
        {
            _outputFile = outputFile ?? throw new ArgumentNullException(nameof(outputFile));
        }

        public async Task Export(ClashResultReader<JObject> reader)
        {
            using (var fout = _outputFile.Open(FileMode.Create))
            using (var sw = new StreamWriter(fout, Encoding.UTF8))
            {
                bool header = false;

                await reader.Read(async obj =>
                {
                    if (!header)
                    {
                        await sw.WriteLineAsync(ToCsvHeader(obj));

                        header = true;
                    }

                    await sw.WriteLineAsync(ToCsvRow(obj));

                    return true;
                });
            }
        }

        private static string ToCsvHeader(JObject line)
        {
            var lineMap = line as IDictionary<string, JToken>;

            return string.Join(",", lineMap.Keys.OrderBy(k => k).Select(k => $"\"{k}\"").ToArray());
        }

        private static string ToCsvRow(JObject line)
        {
            var lineMap = line as IDictionary<string, JToken>;

            return string.Join(",", lineMap.OrderBy(k => k.Key).Select(k => CsvSafeString(k.Value)).ToArray());
        }

        private static string CsvSafeString(JToken value)
        {
            switch (value.Type)
            {
                case JTokenType.String:
                case JTokenType.Date:
                case JTokenType.TimeSpan:
                case JTokenType.Object:
                    return $"\"{(string)value}\"";

                case JTokenType.Null:
                    return "\"\"";

                case JTokenType.Array:
                    return string.Join("~", ((JArray)value).Select(v => CsvSafeString(v)));

                case JTokenType.Raw:
                case JTokenType.Bytes:
                case JTokenType.Comment:
                case JTokenType.Constructor:
                case JTokenType.Undefined:
                    throw new NotSupportedException();

                default:
                    return (string)value;
            }
        }
    }
}
