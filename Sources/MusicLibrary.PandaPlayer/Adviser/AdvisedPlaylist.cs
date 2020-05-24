using System;
using System.Collections.Generic;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Adviser
{
	public class AdvisedPlaylist
	{
		public AdvisedPlaylistType AdvisedPlaylistType { get; private set; }

		public string Title { get; private set; }

		public IReadOnlyCollection<SongModel> Songs { get; private set; }

		private DiscModel disc;

		public DiscModel Disc
		{
			get
			{
				if (disc == null)
				{
					throw new InvalidOperationException("Advise does not have a disc");
				}

				return disc;
			}

			private set => disc = value;
		}

		private AdvisedPlaylist()
		{
		}

		public static AdvisedPlaylist ForDisc(DiscModel disc)
		{
			return new AdvisedPlaylist
			{
				AdvisedPlaylistType = AdvisedPlaylistType.Disc,
				Title = "Stub Title",
				Songs = new List<SongModel>(),
				Disc = disc,
			};
		}
	}
}
