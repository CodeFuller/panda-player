using System;
using PandaPlayer.Core.Models;

namespace PandaPlayer.UnitTests.Extensions
{
	internal static class DiscModelExtensions
	{
		public static DiscModel AddToFolder(this DiscModel disc, FolderModel folder)
		{
			folder.AddDiscs(disc);
			return disc;
		}

		public static DiscModel MakeActive(this DiscModel disc)
		{
			disc.AddSong(new SongModel());
			return disc;
		}

		public static DiscModel MakeDeleted(this DiscModel disc)
		{
			disc.AddSong(new SongModel { DeleteDate = DateTimeOffset.Now });
			return disc;
		}

		public static DiscModel AddSongs(this DiscModel disc, params SongModel[] songs)
		{
			foreach (var song in songs)
			{
				disc.AddSong(song);
			}

			return disc;
		}
	}
}
