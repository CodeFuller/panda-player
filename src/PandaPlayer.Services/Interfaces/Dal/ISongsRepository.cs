using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Interfaces.Dal
{
	public interface ISongsRepository
	{
		Task CreateSong(SongModel song, CancellationToken cancellationToken);

		Task UpdateSong(SongModel song, CancellationToken cancellationToken);

		Task UpdateSongLastPlayback(SongModel song, CancellationToken cancellationToken);
	}
}
