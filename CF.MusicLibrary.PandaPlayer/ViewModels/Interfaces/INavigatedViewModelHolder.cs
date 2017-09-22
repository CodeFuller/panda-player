﻿namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface INavigatedViewModelHolder
	{
		IEditDiscPropertiesViewModel EditDiscPropertiesViewModel { get; }

		IEditSongPropertiesViewModel EditSongPropertiesViewModel { get; }

		IRateDiscViewModel RateDiscViewModel { get; }
	}
}