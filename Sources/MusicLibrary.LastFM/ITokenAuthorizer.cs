using System.Threading.Tasks;

namespace MusicLibrary.LastFM
{
	public interface ITokenAuthorizer
	{
		Task<string> AuthorizeToken(UnauthorizedToken unauthorizedToken);
	}
}
