using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL
{
	public class MusicCatalog : IMusicCatalog
	{
		private readonly IMusicLibraryRepository libraryRepository;

		public MusicCatalog(IMusicLibraryRepository libraryRepository)
		{
			if (libraryRepository == null)
			{
				throw new ArgumentNullException(nameof(libraryRepository));
			}

			this.libraryRepository = libraryRepository;
		}

		public async Task<DiscLibrary> GetDiscsAsync()
		{
			return await libraryRepository.GetDiscLibraryAsync();
		}

		public async Task<IEnumerable<Genre>> GetAllGenresAsync()
		{
			return await libraryRepository.GetGenresAsync();
		}
	}
}
