using System.Collections.Generic;
using System.Collections.ObjectModel;
using MusicLibrary.DiscPreprocessor.AddingToLibrary;

namespace MusicLibrary.DiscPreprocessor.ViewModels.Interfaces
{
	public interface IEditSongsDetailsViewModel : IPageViewModel
	{
		ObservableCollection<SongViewItem> Songs { get; }

		void SetSongs(IEnumerable<AddedSong> songs);
	}
}
