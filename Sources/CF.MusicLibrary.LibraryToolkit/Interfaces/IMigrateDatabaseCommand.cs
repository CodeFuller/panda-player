using System.Threading;
using System.Threading.Tasks;

namespace CF.MusicLibrary.LibraryToolkit.Interfaces
{
	public interface IMigrateDatabaseCommand
	{
		Task Execute(string sourceDatabaseFileName, string targetDatabaseFileName, CancellationToken cancellationToken);
	}
}
