using System.Collections.Generic;
using System.Collections.ObjectModel;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels.Interfaces
{
	public interface IEditSongsDetailsViewModel : IPageViewModel
	{
		ObservableCollection<SongTagDataViewItem> Songs { get; }

		void SetSongs(IEnumerable<TaggedSongData> songs);
	}
}
