using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Extensions;
using MusicLibrary.Services.Comparers;

namespace MusicLibrary.PandaPlayer.Adviser.Extensions
{
	internal static class DiscModelExtensions
	{
		public static IEnumerable<SongModel> GetDiscSongsForAnalysis(this DiscModel disc)
		{
			return disc.IsDeleted ? disc.Songs : disc.Songs.Where(song => !song.IsDeleted);
		}

		public static ArtistModel GetSoloArtist(this DiscModel disc)
		{
			return disc.GetDiscSongsForAnalysis()
				.Select(song => song.Artist)
				.UniqueOrDefault(new ArtistEqualityComparer());
		}
	}
}
