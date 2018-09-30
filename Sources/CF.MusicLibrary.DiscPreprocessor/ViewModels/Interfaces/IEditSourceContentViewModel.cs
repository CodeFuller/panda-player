using System.Collections.Generic;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using CF.MusicLibrary.DiscPreprocessor.ViewModels.SourceContent;

namespace CF.MusicLibrary.DiscPreprocessor.ViewModels.Interfaces
{
	public interface IEditSourceContentViewModel : IPageViewModel
	{
		DiscTreeViewModel CurrentDiscs { get; }

		IEnumerable<AddedDiscInfo> AddedDiscs { get; }

		void LoadDefaultContent();
	}
}
