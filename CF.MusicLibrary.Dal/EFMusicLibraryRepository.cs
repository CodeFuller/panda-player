using System;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.Dal
{
	/// <summary>
	/// Implementation of IMusicLibraryRepository based on Entity Framework.
	/// </summary>
	public class EFMusicLibraryRepository : IMusicLibraryRepository
	{
		/// <summary>
		/// Implementation of IMusicLibraryRepository.LoadLibrary().
		/// </summary>
		public DiscLibrary LoadLibrary()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Implementation of IMusicLibraryRepository.Store().
		/// </summary>
		public void Store(DiscLibrary library)
		{
			if (library == null)
			{
				throw new ArgumentNullException(nameof(library));
			}

			using (var ctx = new MusicLibraryEntities())
			{
				ctx.Configuration.AutoDetectChangesEnabled = false;

				foreach (var disc in library.Discs)
				{
					ctx.Discs.Add(ctx.Discs.ProvideEntity(d => d.Id == disc.Id, () => Disc.CreateDisc(disc, ctx)));
				}

				ctx.SaveChanges();
			}
		}
	}
}
