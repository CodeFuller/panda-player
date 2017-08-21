namespace CF.MusicLibrary.PandaPlayer.Scrobbler
{
	public class UnauthorizedToken
	{
		public string Token { get; }

		public string TokenAuthorizationPage { get; }

		public UnauthorizedToken(string token, string tokenAuthorizationPage)
		{
			Token = token;
			TokenAuthorizationPage = tokenAuthorizationPage;
		}
	}
}
