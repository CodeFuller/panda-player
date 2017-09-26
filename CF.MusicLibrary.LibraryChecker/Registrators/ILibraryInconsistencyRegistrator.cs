using CF.MusicLibrary.BL.Interfaces;

namespace CF.MusicLibrary.LibraryChecker.Registrators
{
	public interface ILibraryInconsistencyRegistrator : IDiscInconsistencyRegistrator, ITagDataInconsistencyRegistrator, ILastFMInconsistencyRegistrator,
		ILibraryStorageInconsistencyRegistrator, IDiscArtInconsistencyRegistrator
	{
	}
}
