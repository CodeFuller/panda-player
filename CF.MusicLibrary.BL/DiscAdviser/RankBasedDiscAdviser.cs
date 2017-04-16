using System;
using System.Collections.ObjectModel;
using System.Linq;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL.DiscAdviser
{
	public class RankBasedDiscAdviser : IDiscAdviser
	{
		private readonly IRankCalculator rankCalculator;

		public RankBasedDiscAdviser(IRankCalculator rankCalculator)
		{
			this.rankCalculator = rankCalculator;
		}

		public Collection<LibraryDisc> AdviseNextDiscs(ArtistLibrary library, int discsNumber)
		{
			if (library == null)
			{
				throw new ArgumentNullException(nameof(library));
			}

			Collection<LibraryDisc> advisedDiscs = new Collection<LibraryDisc>();
			foreach (var artist in library.Artists.OrderByDescending(a => rankCalculator.CalculateArtistRank(a)))
			{
				var artistdDisc = artist.Discs.OrderByDescending(d => rankCalculator.CalculateDiscRank(d)).FirstOrDefault();
				if (artistdDisc != null)
				{
					advisedDiscs.Add(artistdDisc);
					if (advisedDiscs.Count >= discsNumber)
					{
						break;
					}
				}
			}

			return advisedDiscs;
		}
	}
}
