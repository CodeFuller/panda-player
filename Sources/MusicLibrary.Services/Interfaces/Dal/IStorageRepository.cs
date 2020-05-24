using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Interfaces.Dal
{
	public interface IStorageRepository
	{
		Task UpdateSong(SongModel song, CancellationToken cancellationToken);
	}
}
