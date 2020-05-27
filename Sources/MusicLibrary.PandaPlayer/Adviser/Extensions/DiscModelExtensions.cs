using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.Comparers;
using MusicLibrary.Services.Extensions;

namespace MusicLibrary.PandaPlayer.Adviser.Extensions
{
	internal static class DiscModelExtensions
	{
		public static IEnumerable<SongModel> GetDiscSongsForAnalysis(this DiscModel disc)
		{
			return disc.IsDeleted ? disc.AllSongs : disc.ActiveSongs;
		}

		public static ArtistModel GetSoloArtist(this DiscModel disc)
		{
			return disc.GetDiscSongsForAnalysis()
				.Select(song => song.Artist)
				.UniqueOrDefault(new ArtistEqualityComparer());
		}
	}
}
