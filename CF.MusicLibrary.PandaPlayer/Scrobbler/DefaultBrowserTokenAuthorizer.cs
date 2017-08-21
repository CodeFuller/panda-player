using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CF.MusicLibrary.PandaPlayer.Scrobbler
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
