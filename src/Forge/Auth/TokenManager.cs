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
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Forge.Auth
{
    internal sealed class TokenManager : ITokenManager
    {
        private readonly SampleConfiguration _configuration;

        private Token _token;

        public TokenManager(SampleConfiguration configuration) => _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

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
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"https://{_configuration.Host}/authentication/v1/gettoken"))
            {
                var body = $"client_id={_configuration.ClientId}&client_secret={_configuration.Secret}&grant_type=authorization_code&code={code}&redirect_uri={_configuration.CallbackUrl}";

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
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"https://{_configuration.Host}/authentication/v1/refreshtoken"))
            {
                var body = $"client_id={_configuration.ClientId}&client_secret={_configuration.Secret}&grant_type=refresh_token&refresh_token={_token.Refresh}";

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
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"https://{_configuration.Host}/validation/v1/validatetoken"))
            {
                var body = $"client_id={_configuration.ClientId}&client_secret={_configuration.Secret}&grant_type=urn:pingidentity.com:oauth2:validated_token&token={token}";

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
