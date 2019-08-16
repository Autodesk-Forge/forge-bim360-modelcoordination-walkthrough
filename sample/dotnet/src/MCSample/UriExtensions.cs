using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MCSample
{
    public static class UriExtensions
    {
        public static async Task<Stream> OpenHttpStream(this Uri address, IDictionary<string, string> headers, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, address))
            {
                foreach (var kvp in headers)
                {
                    request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
                }

                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(request, cancellationToken);

                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStreamAsync();
                }
            }
        }

        public static async Task<dynamic> HttpGetDynamicJson(this Uri address, IDictionary<string, string> headers, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, address))
            {
                foreach (var kvp in headers)
                {
                    request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
                }

                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(request, cancellationToken);

                    response.EnsureSuccessStatusCode();

                    return JObject.Parse((await response.Content.ReadAsStringAsync()));
                }
            }
        }
    }
}
