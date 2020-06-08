using System.Collections.Generic;
using MusicLibrary.DiscPreprocessor.MusicStorage;
using MusicLibrary.DiscPreprocessor.ViewModels.SourceContent;

namespace MusicLibrary.DiscPreprocessor.ViewModels.Interfaces
{
	public interface IEditSourceContentViewModel : IPageViewModel
	{
		DiscTreeViewModel CurrentDiscs { get; }

		IEnumerable<AddedDiscInfo> AddedDiscs { get; }

		void LoadDefaultContent();
	}
}
