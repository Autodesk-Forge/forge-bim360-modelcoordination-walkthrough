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
using System;
using System.IO;

namespace Sample.Forge
{
    public class ContentChunk : IDisposable
    {
        private readonly string _contentRange;

        private MemoryStream _chunk;

        private bool _disposed = false;

        public ContentChunk(string contentRange, MemoryStream chunkStream)
        {
            if (string.IsNullOrWhiteSpace(contentRange))
            {
                throw new ArgumentException("message", nameof(contentRange));
            }

            _contentRange = contentRange;

            _chunk = chunkStream ?? throw new ArgumentNullException(nameof(chunkStream));

            _chunk.Seek(0, SeekOrigin.Begin);
        }

        public string Range => _contentRange;

        public Stream Content => _chunk;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        if (_chunk != null)
                        {
                            _chunk.Dispose();
                            _chunk = null;
                        }
                    }
                }
                finally
                {
                    _disposed = true;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
