using Autodesk.Forge.Bim360.ModelCoordination.Index;
using System.Security.Cryptography;
using System.Text;

namespace MCSample.Model
{
    internal static class IndexQueryExtensions
    {
        public static string GetThumbprint(this IndexQuery query)
        {
            using (var hashFunction = MD5.Create())
            {
                var hash = hashFunction.ComputeHash(Encoding.UTF8.GetBytes(query.Statement.ToUpperInvariant()));

                var sb = new StringBuilder(2 * hash.Length);

                foreach (byte b in hash)
                {
                    sb.AppendFormat("{0:X2}", b);
                }

                return sb.ToString();
            }
        }
    }
}
