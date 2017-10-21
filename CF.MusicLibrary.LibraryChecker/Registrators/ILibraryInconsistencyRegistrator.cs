using CF.MusicLibrary.Core.Interfaces;

namespace CF.MusicLibrary.LibraryChecker.Registrators
{
	public interface ILibraryInconsistencyRegistrator : IDiscInconsistencyRegistrator, ITagDataInconsistencyRegistrator, ILastFMInconsistencyRegistrator,
		ILibraryStorageInconsistencyRegistrator, IDiscImageInconsistencyRegistrator
	{
	}
}
