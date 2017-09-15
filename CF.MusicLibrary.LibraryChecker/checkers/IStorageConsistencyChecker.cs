using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public interface IStorageConsistencyChecker
	{
		Task CheckStorage(DiscLibrary library);
	}
}
