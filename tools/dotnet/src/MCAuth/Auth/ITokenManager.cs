using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAuth.Auth
{
    internal interface ITokenManager
    {
        Task<Token> GetAccessToken(bool forceRefresh = false);

        Task<string> GetAccessTokenString(bool forceRefresh = false);

        Task<ValidationToken> ValidateToken();

        Task Configure(string code);
    }
}
