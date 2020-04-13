using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MusicLibrary.LastFM
{
	public class DefaultBrowserTokenAuthorizer : ITokenAuthorizer
	{
		public async Task<string> AuthorizeToken(UnauthorizedToken unauthorizedToken)
		{
			Process.Start(unauthorizedToken.TokenAuthorizationPage);
			await Task.Delay(TimeSpan.FromSeconds(10));

			return unauthorizedToken.Token;
		}
	}
}
