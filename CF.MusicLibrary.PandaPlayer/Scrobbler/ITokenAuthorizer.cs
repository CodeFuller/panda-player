using System.Threading.Tasks;

namespace CF.MusicLibrary.PandaPlayer.Scrobbler
{
	public interface ITokenAuthorizer
	{
		Task<string> AuthorizeToken(UnauthorizedToken unauthorizedToken);
	}
}
