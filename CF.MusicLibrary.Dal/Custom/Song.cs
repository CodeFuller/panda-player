using System;
using System.Linq;

namespace CF.MusicLibrary.Dal
{
	/// <summary>
	/// Class for Song entity.
	/// </summary>
	public partial class Song
	{
		/// <summary>
		/// Constructs Song entity from BL.Objects.Song.
		/// </summary>
		public static Song CreateSong(BL.Objects.Song song, MusicLibraryEntities ctx)
		{
			if (song == null)
			{
				throw new ArgumentNullException(nameof(song));
			}

			return new Song
			{
				Id = song.Id,
				Artist = ctx.Artists.ProvideEntity(a => a.Id == song.Artist.Id, () => Artist.CreateArtist(song.Artist)),
				OrderNumber = song.OrderNumber,
				Year = song.Year,
				Title = song.Title,
				Genre = ctx.Genres.ProvideEntity(g => g.Id == song.Genre.Id, () => Genre.CreateGenre(song.Genre)),
				Duration = (int)song.Duration.TotalMilliseconds,
				Rating = (byte?)song.Rating,
				Uri = song.Uri.ToString(),
				FileSize = song.FileSize,
				Bitrate = song.Bitrate,
				LastPlaybackTime = song.LastPlaybackTime,
				PlaybacksCount = song.PlaybacksCount,
				Playbacks = song.Playbacks.Select(Playback.CreatePlayback).ToList(),
			};
		}
	}
}
