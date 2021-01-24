using System.Collections.Generic;
using System.Collections.ObjectModel;
using MusicLibrary.DiscAdder.AddedContent;
using MusicLibrary.DiscAdder.ViewModels.ViewModelItems;

namespace MusicLibrary.DiscAdder.ViewModels.Interfaces
{
	internal interface IEditSongsDetailsViewModel : IPageViewModel
	{
		ObservableCollection<SongViewItem> Songs { get; }

		void SetSongs(IEnumerable<AddedSong> songs);
	}
}
