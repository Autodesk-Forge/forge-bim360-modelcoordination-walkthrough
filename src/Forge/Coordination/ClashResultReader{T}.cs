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
using System.Text;
using System.Threading.Tasks;

namespace Sample.Forge.Coordination
{
    public class ClashResultReader<T>
        where T : class, new()
    {
        private readonly Stream _stream;

        private readonly FileInfo _file = null;

        private readonly bool _decompress = false;

        public ClashResultReader(Stream stream, bool decompress = true)
        {
            _stream = stream ?? throw new System.ArgumentNullException(nameof(stream));

            _decompress = decompress;
        }

        public ClashResultReader(FileInfo file, bool decompress = true)
        {
            _file = file ?? throw new System.ArgumentNullException(nameof(file));

            _decompress = decompress;
        }

        public async Task Read(Func<T, Task<bool>> processor)
        {
            bool ownsStream;
            Stream inputStream = null;

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

            GZipStream dc = null;
            StreamReader sr = null;

            try
            {
                if (_decompress)
                {
                    dc = new GZipStream(inputStream, CompressionMode.Decompress);
                    sr = new StreamReader(dc, Encoding.UTF8);
                }
                else
                {
                    sr = new StreamReader(inputStream, Encoding.UTF8);
                }

                using (var reader = new JsonTextReader(sr))
                {
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.StartArray)
                        {
                            while (await reader.ReadAsync())
                            {
                                if (reader.TokenType == JsonToken.StartObject)
                                {
                                    var obj = JObject.Load(reader);

                                    if (typeof(T) == typeof(JObject))
                                    {
                                        if (await processor((T)(object)obj) == false)
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (await processor(obj.ToObject<T>()) == false)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                try
                {
                    if (sr != null)
                    {
                        sr.Close();
                        sr.Dispose();
                        sr = null;
                    }
                }
                finally
                {
                    try
                    {
                        if (dc != null)
                        {
                            dc.Close();
                            dc.Dispose();
                            dc = null;
                        }
                    }
                    finally
                    {
                        if (ownsStream)
                        {
                            inputStream.Close();
                            inputStream.Dispose();
                        }
                    }
                }
            }
        }
    }
}
