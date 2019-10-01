using MCAuth.Auth;
using MCCommon;
using System;
using System.Composition;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace MCAuth
{
    [Export]
    internal sealed class SignInViewModel
    {
        private readonly ITokenManager _tokenManager;

        [ImportingConstructor]
        public SignInViewModel(ITokenManager tokenManager)
        {
            _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));
        }

        public NavigationService NavigationService { get; set; }

        public string AuthUrl => ForgeAppConfiguration.Current.AuthorizeUrlCode.AbsoluteUri;

        public async Task SignInSuccess(string code)
        {
            await _tokenManager.Configure(code);

            this.NavigationService.Navigate(new TokenManagerView());
        }
    }
}
