﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer
{
	public interface IViewNavigator
	{
		void BringApplicationToFront();

		void ShowRateDiscView(Disc disc);

		void ShowDiscPropertiesView(Disc disc);

		void ShowSongPropertiesView(IEnumerable<Song> songs);

		Task ShowEditDiscArtView(Disc disc);

		void ShowLibraryStatisticsView();
	}
}
