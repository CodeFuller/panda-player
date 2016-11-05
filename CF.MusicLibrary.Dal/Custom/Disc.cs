using System;
using System.Linq;

namespace CF.MusicLibrary.Dal
{
	/// <summary>
	/// Class for Disc entity.
	/// </summary>
	public partial class Disc
	{
		/// <summary>
		/// Constructs Disc entity from BL.Objects.Disc.
		/// </summary>
		public static Disc CreateDisc(BL.Objects.Disc disc, MusicLibraryEntities ctx)
		{
			if (disc == null)
			{
				throw new ArgumentNullException(nameof(disc));
			}

			return new Disc
			{
				Id = disc.Id,
				Title = disc.Title,
				Uri = disc.Uri.ToString(),
				Songs = disc.Songs.Select(s => Song.CreateSong(s, ctx)).ToList(),
			};
		}
	}
}
