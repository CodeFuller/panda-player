﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface ILibraryExplorerViewModel : INotifyPropertyChanged
	{
		ObservableCollection<FolderExplorerItem> Items { get; }

		FolderExplorerItem SelectedItem { get; set; }

		void Load();
	}
}