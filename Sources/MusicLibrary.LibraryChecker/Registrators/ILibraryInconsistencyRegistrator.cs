using MusicLibrary.Core.Interfaces;

namespace MusicLibrary.LibraryChecker.Registrators
{
	public interface ILibraryInconsistencyRegistrator : IDiscInconsistencyRegistrator, ITagDataInconsistencyRegistrator, ILastFMInconsistencyRegistrator,
		ILibraryStorageInconsistencyRegistrator, IDiscImageInconsistencyRegistrator
	{
	}
}
