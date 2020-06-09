using System.Collections.Generic;
using MusicLibrary.DiscAdder.MusicStorage;
using MusicLibrary.DiscAdder.ViewModels.SourceContent;

namespace MusicLibrary.DiscAdder.ViewModels.Interfaces
{
	internal interface IEditSourceContentViewModel : IPageViewModel
	{
		DiscTreeViewModel CurrentDiscs { get; }

		IEnumerable<AddedDiscInfo> AddedDiscs { get; }

		void LoadDefaultContent();
	}
}
