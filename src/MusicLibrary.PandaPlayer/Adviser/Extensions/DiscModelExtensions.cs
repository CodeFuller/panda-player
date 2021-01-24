using System.Collections.Generic;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Adviser.Extensions
{
	internal static class DiscModelExtensions
	{
		public static IEnumerable<SongModel> GetDiscSongsForAnalysis(this DiscModel disc)
		{
			return disc.IsDeleted ? disc.AllSongs : disc.ActiveSongs;
		}
	}
}
