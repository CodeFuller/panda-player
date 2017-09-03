using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels.Interfaces
{
	public interface IEditSongsDetailsViewModel : IPageViewModel
	{
		ObservableCollection<SongTagDataViewItem> Songs { get; }

		void SetSongs(IEnumerable<TaggedSongData> songs);
	}
}
