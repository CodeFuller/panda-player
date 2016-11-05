using System;

namespace CF.MusicLibrary.Dal
{
	/// <summary>
	/// Class for Artist entity.
	/// </summary>
	public partial class Artist
	{
		/// <summary>
		/// Constructs Artist entity from BL.Objects.Artist.
		/// </summary>
		public static Artist CreateArtist(BL.Objects.Artist artist)
		{
			if (artist == null)
			{
				throw new ArgumentNullException(nameof(artist));
			}

			return new Artist
			{
				Id = artist.Id,
				Name = artist.Name,
			};
		}
	}
}
