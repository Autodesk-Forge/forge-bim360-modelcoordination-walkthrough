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
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Forge
{
    public static class FileInfoExtensions
    {
        public const int MaxChunkSize = 5 * 1024 * 1024;

        public static ContentDispositionHeaderValue GetFileContentDisposition(this FileInfo file)
        {
            file.Refresh();

            return new ContentDispositionHeaderValue("attachment")
            {
                FileNameStar = file.Name,
                FileName = file.Name,
                CreationDate = file.CreationTimeUtc,
                ModificationDate = file.LastWriteTimeUtc,
                ReadDate = file.LastAccessTimeUtc,
                Size = file.Length
            };
        }

        public static string GetSha1(this FileInfo file)
        {
            using (var fs = file.OpenRead())
            using (var sha1 = SHA1Managed.Create())
            {
                byte[] hash = sha1.ComputeHash(fs);

                var sb = new StringBuilder(2 * hash.Length);

                foreach (byte b in hash)
                {
                    sb.AppendFormat("{0:X2}", b);
                }

                return sb.ToString();
            }
        }

        public static async Task<List<ContentChunk>> GetContentChunks(this FileInfo file, int maxChunkSize = MaxChunkSize)
        {
            var res = new List<ContentChunk>();

            file.Refresh();

            int offset = 0;
            int read = -1;
            int readTotal = 0;

            var buffer = new byte[maxChunkSize];

            using (var fin = file.OpenRead())
            {
                while ((read = await fin.ReadAsync(buffer, 0, maxChunkSize)) > 0)
                {
                    readTotal += read;

                    var ms = new MemoryStream();

                    await ms.WriteAsync(buffer, 0, read);

                    var cc = new ContentChunk($"bytes {offset}-{readTotal - 1}/{file.Length}", ms);

                    res.Add(cc);

                    offset += read;
                }
            }

            return res;
        }
    }
}
