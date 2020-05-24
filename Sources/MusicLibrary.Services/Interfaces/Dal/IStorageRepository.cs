using System;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Interfaces.Dal
{
	public interface IStorageRepository
	{
		Task UpdateSongTreeTitle(SongModel newSong, Uri currentSongContentUri, CancellationToken cancellationToken);

		Task UpdateSong(SongModel song, CancellationToken cancellationToken);
	}
}
