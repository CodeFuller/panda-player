using System.Collections.Generic;
using System.Collections.ObjectModel;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;

namespace CF.MusicLibrary.DiscPreprocessor.ViewModels.Interfaces
{
	public interface IEditSongsDetailsViewModel : IPageViewModel
	{
		ObservableCollection<SongViewItem> Songs { get; }

		void SetSongs(IEnumerable<AddedSong> songs);
	}
}
