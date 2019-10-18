using MCCommon;
using Newtonsoft.Json;
using System;
using System.Composition;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MCCommon.Auth
{
    [Shared]
    [Export(typeof(ITokenManager))]
    internal sealed class TokenManager : ITokenManager
    {
        private Token _token;
      
        public TokenManager()
        {
        }

        public bool Configured => _token != null;

        public async Task<Token> GetAccessToken(bool forceRefresh = false)
        {
            if (_token == null)
            {
                throw new InvalidOperationException("Error, null token, have you called configure?");
            }

            if (DateTime.UtcNow >= _token.ExpiresOn || forceRefresh)
            {
                await RefreshToken();
            }

            return _token;
        }

        public async Task<string> GetAccessTokenString(bool forceRefresh = false)
        {
            var token = await GetAccessToken(forceRefresh);

            return token.AccessToken;
        }

        public async Task Configure(string code)
        {
            _token = null;

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"https://{ForgeAppConfiguration.Current.Host}/authentication/v1/gettoken"))
            {
                var body = $"client_id={ForgeAppConfiguration.Current.ClientId}&client_secret={ForgeAppConfiguration.Current.Secret}&grant_type=authorization_code&code={code}&redirect_uri={ForgeAppConfiguration.Current.CallbackUrl}";

                request.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");

                using (var resp = await client.SendAsync(request))
                {
                    if (resp.IsSuccessStatusCode)
                    {
                        var json = await resp.Content.ReadAsStringAsync();

                        _token = JsonConvert.DeserializeObject<Token>(json);

                        _token.ExpiresOn = DateTime.UtcNow.AddSeconds(_token.ExpiresIn) - TimeSpan.FromMinutes(10);
                    }
                }
            }
        }

        private async Task RefreshToken()
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"https://{ForgeAppConfiguration.Current.Host}/authentication/v1/refreshtoken"))
            {
                var body = $"client_id={ForgeAppConfiguration.Current.ClientId}&client_secret={ForgeAppConfiguration.Current.Secret}&grant_type=refresh_token&refresh_token={_token.Refresh}";

                request.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");

                using (var resp = await client.SendAsync(request))
                {
                    if (resp.IsSuccessStatusCode)
                    {
                        var json = await resp.Content.ReadAsStringAsync();

                        _token = JsonConvert.DeserializeObject<Token>(json);

                        _token.ExpiresOn = DateTime.UtcNow.AddSeconds(_token.ExpiresIn) - TimeSpan.FromMinutes(5); ;
                    }
                    else
                    {
                        throw new InvalidOperationException("Error, token refresh failed.");
                    }
                }
            }
        }

        public async Task<ValidationToken> ValidateToken()
        {
            ValidationToken vt = null;

            var token = await this.GetAccessTokenString();

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"https://{ForgeAppConfiguration.Current.Host}/validation/v1/validatetoken"))
            {
                var body = $"client_id={ForgeAppConfiguration.Current.ClientId}&client_secret={ForgeAppConfiguration.Current.Secret}&grant_type=urn:pingidentity.com:oauth2:validated_token&token={token}";

                request.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");

                using (var resp = await client.SendAsync(request))
                {
                    if (resp.IsSuccessStatusCode)
                    {
                        var json = await resp.Content.ReadAsStringAsync();

                        vt = JsonConvert.DeserializeObject<ValidationToken>(json);
                    }
                    else
                    {
                        throw new InvalidOperationException("Error, token refresh failed.");
                    }
                }
            }

            return vt;
        }
    }
}
