using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCCommon.Auth
{
    public interface ITokenManager
    {
        bool Configured { get; }

        Task<Token> GetAccessToken(bool forceRefresh = false);

        Task<string> GetAccessTokenString(bool forceRefresh = false);

        Task<ValidationToken> ValidateToken();

        Task Configure(string code);
    }
}
