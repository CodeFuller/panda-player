using System.Threading;
using System.Threading.Tasks;

namespace MusicLibrary.LibraryToolkit.Interfaces
{
	public interface ISeedApiDatabaseCommand
	{
		Task Execute(CancellationToken cancellationToken);
	}
}
