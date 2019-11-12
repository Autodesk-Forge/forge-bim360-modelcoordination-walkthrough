/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Forge.Coordination
{
    public class IndexResultReader
    {
        private readonly FileInfo _file;

        private readonly Stream _stream;

        private readonly IReadOnlyDictionary<string, IndexField> _fields;

        public IndexResultReader(Stream stream, IReadOnlyDictionary<string, IndexField> fields = null)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));

            _fields = fields;
        }

        public IndexResultReader(FileInfo file, IReadOnlyDictionary<string, IndexField> fields = null)
        {
            _file = file ?? throw new ArgumentNullException(nameof(file));

            _fields = fields;
        }

        public async Task<QueryResultSummary> ReadToEndAsync(Func<JObject, Task<bool>> rowProcessor = null, bool substituteFieldkeys = true)
        {
            if (substituteFieldkeys && _fields == null)
            {
                throw new InvalidOperationException("substituteFieldkeys = true not supported for instances of IndexResultReader(file, null);");
            }

            var fields = new Dictionary<string, IndexField>(StringComparer.OrdinalIgnoreCase);

            uint count = 0;

            Stream inputStream = null;
            bool ownsStream;

            if (_file != null)
            {
                inputStream = _file.OpenRead();
                ownsStream = true;
            }
            else
            {
                inputStream = _stream;
                ownsStream = false;
            }

            try
            {
                using (var gzip = new GZipStream(inputStream, CompressionMode.Decompress))
                using (var sr = new StreamReader(gzip, Encoding.UTF8))
                using (var jr = new JsonTextReader(sr))
                {
                    jr.SupportMultipleContent = true;

                    while (await jr.ReadAsync())
                    {
                        var obj = await JObject.LoadAsync(jr);

                        count++;

                        if (substituteFieldkeys)
                        {
                            foreach (var token in obj.DescendantsAndSelf().ToArray())
                            {
                                var property = token as JProperty;

                                if (property != null)
                                {
                                    if (_fields.ContainsKey(property.Name))
                                    {
                                        if (!fields.ContainsKey(property.Name))
                                        {
                                            fields[property.Name] = _fields[property.Name];
                                        }

                                        try
                                        {
                                            property.Replace(new JProperty(_fields[property.Name].Name, property.Value));
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                        }
                                    }
                                }
                            }
                        }

                        if (rowProcessor != null)
                        {
                            if (!(await rowProcessor(obj)))
                            {
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (ownsStream && inputStream != null)
                {
                    inputStream.Close();
                    inputStream.Dispose();
                    inputStream = null;
                }
            }

            return new QueryResultSummary
            {
                Fields = fields,
                RowCount = count
            };
        }
    }
}

