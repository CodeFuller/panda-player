using System;

namespace CF.MusicLibrary.Dal
{
	/// <summary>
	/// Class for Genre entity.
	/// </summary>
	public partial class Genre
	{
		/// <summary>
		/// Constructs Genre entity from BL.Objects.Genre.
		/// </summary>
		public static Genre CreateGenre(BL.Objects.Genre genre)
		{
			if (genre == null)
			{
				throw new ArgumentNullException(nameof(genre));
			}

			return new Genre
			{
				Id = genre.Id,
				Name = genre.Name,
			};
		}
	}
}
