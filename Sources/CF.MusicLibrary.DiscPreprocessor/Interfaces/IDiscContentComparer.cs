using CF.MusicLibrary.DiscPreprocessor.ViewModels.SourceContent;

namespace CF.MusicLibrary.DiscPreprocessor.Interfaces
{
	public interface IDiscContentComparer
	{
		void SetDiscsCorrectness(DiscTreeViewModel ethalonDiscs, DiscTreeViewModel currentDiscs);
	}
}
