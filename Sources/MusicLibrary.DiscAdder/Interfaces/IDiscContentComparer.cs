﻿using MusicLibrary.DiscAdder.ViewModels.SourceContent;

namespace MusicLibrary.DiscAdder.Interfaces
{
	internal interface IDiscContentComparer
	{
		void SetDiscsCorrectness(DiscTreeViewModel referenceDiscs, DiscTreeViewModel currentDiscs);
	}
}
