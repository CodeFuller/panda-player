using System.Threading.Tasks;

namespace CF.MusicLibrary.LastFM
{
	public interface ITokenAuthorizer
	{
		Task<string> AuthorizeToken(UnauthorizedToken unauthorizedToken);
	}
}
