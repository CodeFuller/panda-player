﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Interfaces.Dal
{
	public interface IStorageRepository
	{
		Task UpdateSongTreeTitle(SongModel oldSong, SongModel newSong, CancellationToken cancellationToken);

		Task UpdateSong(SongModel song, CancellationToken cancellationToken);

		Task DeleteSong(SongModel song, CancellationToken cancellationToken);

		Task AddDiscImage(DiscImageModel image, Stream imageContent, CancellationToken cancellationToken);

		Task DeleteDiscImage(DiscImageModel image, CancellationToken cancellationToken);
	}
}
