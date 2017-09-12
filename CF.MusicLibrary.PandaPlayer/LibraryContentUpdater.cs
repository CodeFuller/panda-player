using System;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer
{
	public class LibraryContentUpdater : ILibraryContentUpdater
	{
		private readonly IMusicLibrary musicLibrary;

		public LibraryContentUpdater(IMusicLibrary musicLibrary)
		{
			if (musicLibrary == null)
			{
				throw new ArgumentNullException(nameof(musicLibrary));
			}

			this.musicLibrary = musicLibrary;
		}

		public async Task DeleteDisc(Disc disc)
		{
			await musicLibrary.DeleteDisc(disc);
		}
	}
}
