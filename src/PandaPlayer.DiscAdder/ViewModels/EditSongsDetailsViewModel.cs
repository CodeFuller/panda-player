using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using PandaPlayer.DiscAdder.ViewModels.Interfaces;
using PandaPlayer.DiscAdder.ViewModels.ViewModelItems;

namespace PandaPlayer.DiscAdder.ViewModels
{
	internal class EditSongsDetailsViewModel : ViewModelBase, IEditSongsDetailsViewModel
	{
		public string Name => "Edit Songs Details";

		public bool DataIsReady => Songs.Any();

		public ObservableCollection<SongViewItem> Songs { get; private set; }

		public void Load(IEnumerable<DiscViewItem> discs)
		{
			var songs = discs.SelectMany(disc => disc.AddedSongs.Select(song => new SongViewItem(disc, song)));
			Songs = new ObservableCollection<SongViewItem>(songs);
		}
	}
}
