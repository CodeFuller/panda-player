using MusicLibrary.Dal.LocalDb.Internal;

namespace MusicLibrary.Dal.LocalDb.Interfaces
{
	internal interface IMusicLibraryDbContextFactory
	{
		MusicLibraryDbContext Create();
	}
}
