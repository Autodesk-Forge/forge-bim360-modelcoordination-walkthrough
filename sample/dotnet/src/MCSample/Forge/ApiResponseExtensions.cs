using Autodesk.Forge.Client;

namespace MCSample.Forge
{
    public static class ApiResponseExtensions
    {
        public static string LocationHeader(this ApiResponse<object> response) => response.ValueFromHeader("Location");

        public static string ValueFromHeader(this ApiResponse<object> response, string key) => response.Headers != null && response.Headers.ContainsKey(key) ? response.Headers[key] : default;
    }
}
