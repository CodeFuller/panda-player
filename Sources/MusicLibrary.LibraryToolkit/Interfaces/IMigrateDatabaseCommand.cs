using System.Threading;
using System.Threading.Tasks;

namespace MusicLibrary.LibraryToolkit.Interfaces
{
	public interface IMigrateDatabaseCommand
	{
		Task Execute(string sourceDatabaseFileName, string targetDatabaseFileName, CancellationToken cancellationToken);
	}
}
