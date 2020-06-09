using System.Collections.Generic;
using System.Collections.ObjectModel;
using MusicLibrary.DiscAdder.AddingToLibrary;

namespace MusicLibrary.DiscAdder.ViewModels.Interfaces
{
	public interface IEditSongsDetailsViewModel : IPageViewModel
	{
		ObservableCollection<SongViewItem> Songs { get; }

		void SetSongs(IEnumerable<AddedSong> songs);
	}
}
