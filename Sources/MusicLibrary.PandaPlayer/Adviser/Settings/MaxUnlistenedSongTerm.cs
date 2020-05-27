﻿using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Adviser.Settings
{
	internal class MaxUnlistenedSongTerm
	{
		public RatingModel Rating { get; set; }

		public int Days { get; set; }
	}
}
