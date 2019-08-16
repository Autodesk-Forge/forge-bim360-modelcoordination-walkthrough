using System;
using System.IO;

namespace MCSample
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
