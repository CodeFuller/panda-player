using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IEditSongPropertiesViewModel
	{
		bool SingleSongMode { get; }

		public string Title { get; set; }

		public EditedSongProperty<ArtistModel> Artist { get; set; }

		public EditedSongProperty<GenreModel> Genre { get; set; }

		public short? TrackNumber { get; set; }

		Task Load(IEnumerable<SongModel> songs, CancellationToken cancellationToken);

		Task Save(CancellationToken cancellationToken);
	}
}
