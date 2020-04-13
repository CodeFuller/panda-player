using MusicLibrary.DiscPreprocessor.ViewModels.SourceContent;

namespace MusicLibrary.DiscPreprocessor.Interfaces
{
	public interface IDiscContentComparer
	{
		void SetDiscsCorrectness(DiscTreeViewModel ethalonDiscs, DiscTreeViewModel currentDiscs);
	}
}
