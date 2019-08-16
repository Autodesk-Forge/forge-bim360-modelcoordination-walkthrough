using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSample.Model
{
    public sealed class IndexResultCsvExporter
    {
        private readonly IReadOnlyDictionary<string, IndexField> _fields;
        private readonly FileInfo _resultFile;
        private readonly FileInfo _outputFile;

        public IndexResultCsvExporter(IReadOnlyDictionary<string, IndexField> fields, FileInfo resultFile, FileInfo outputFile)
        {
            _fields = fields ?? throw new System.ArgumentNullException(nameof(fields));
            _resultFile = resultFile ?? throw new System.ArgumentNullException(nameof(resultFile));
            _outputFile = outputFile ?? throw new System.ArgumentNullException(nameof(outputFile));
        }

        public event EventHandler<PctPorcessedEventArgs> ExportProgress;

        public async Task Export()
        {
            _resultFile.Refresh();

            if (!_resultFile.Exists)
            {
                throw new FileNotFoundException(_resultFile.FullName);
            }

            var reader = new IndexResultReader(_resultFile, _fields);

            // a bit inefficient but scan the file first and buid up a set of columns
            var summary = await reader.ReadToEndAsync();

            using (var fs = _outputFile.Open(FileMode.Create))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
            {
                var fields = summary.Fields.Values.ToArray();

                for (int i = 0; i < fields.Length; i++)
                {
                    var header = (!string.IsNullOrWhiteSpace(fields[i].Category) ? fields[i].Category + "~" : string.Empty) +
                                 fields[i].Type.ToString() +
                                 (!string.IsNullOrWhiteSpace(fields[i].Name) ? "~" + fields[i].Name : string.Empty) +
                                 (!string.IsNullOrWhiteSpace(fields[i].Uom) ? "~" + fields[i].Uom : string.Empty);

                    await sw.WriteAsync(header);

                    if (i != fields.Length - 1)
                    {
                        await sw.WriteAsync(",");
                    }
                    else
                    {
                        await sw.WriteAsync(Environment.NewLine);
                    }
                }

                var progress = ExportProgress;
                var progressNudge = Math.Round((double)summary.RowCount / 100);
                var progressPct = 0U;

                using (var rs = _resultFile.OpenRead())
                using (var rsgz = new GZipStream(rs, CompressionMode.Decompress))
                using (var sr = new StreamReader(rsgz, Encoding.UTF8))
                {
                    uint rowNumber = 0;

                    await reader.ReadToEndAsync(async (row) =>
                    {
                        var obj = row.ToObject<Dictionary<string, object>>();

                        rowNumber++;

                        for (int i = 0; i < fields.Length; i++)
                        {
                            if (obj.ContainsKey(fields[i].Key))
                            {
                                await sw.WriteAsync(GetFieldValue(fields[i], obj[fields[i].Key]));
                            }
                            else
                            {
                                await sw.WriteAsync(GetFieldValue(fields[i], null));
                            }

                            if (i != fields.Length - 1)
                            {
                                await sw.WriteAsync(",");
                            }
                            else
                            {
                                await sw.WriteAsync(Environment.NewLine);
                            }
                        }

                        if (progress != null)
                        {
                            if (rowNumber % progressNudge == 0)
                            {
                                progress.Invoke(this, new PctPorcessedEventArgs(++progressPct));
                            }
                        }

                        return true;
                    }, false);
                }
            }
        }


        public static string GetFieldValue(IndexField field, object value)
        {
            string res = null;

            switch (field.Type)
            {
                case IndexFieldType.Integer:
                    {
                        if (value != null)
                        {
                            if (field.Category.Equals("nucleus", StringComparison.OrdinalIgnoreCase) &&
                                field.Name.Equals("checksum", StringComparison.OrdinalIgnoreCase))
                            {
                                res = BitConverter.ToInt32(Encoding.ASCII.GetBytes((string)value), 0).ToString();
                            }
                            else
                            {
                                res = value.ToString();
                            }
                        }
                        else
                        {
                            res = "0";
                        }
                    }
                    break;

                case IndexFieldType.Double:
                    {
                        if (value != null)
                        {
                            res = value.ToString();
                        }
                        else
                        {
                            res = "0.0";
                        }
                    }
                    break;

                case IndexFieldType.DbKey:
                    {
                        if (value != null)
                        {
                            res = value.ToString();
                        }
                        else
                        {
                            res = "-1";
                        }
                    }
                    break;

                case IndexFieldType.Boolean:
                    {
                        if (value != null)
                        {
                            res = (bool)value ? "1" : "0";
                        }
                        else
                        {
                            res = "0";
                        }
                    }
                    break;

                default:
                    {
                        if (value != null)
                        {
                            // escape quotes RFC-4180
                            res = "\"" + value.ToString().Replace("\"", "\"\"") + "\"";
                        }
                        else
                        {
                            res = "\"\"";
                        }
                    }
                    break;
            }

            return res;
        }
    }
}
