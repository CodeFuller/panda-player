using MusicLibrary.DiscAdder.ViewModels.SourceContent;

namespace MusicLibrary.DiscAdder.Interfaces
{
	public interface IDiscContentComparer
	{
		void SetDiscsCorrectness(DiscTreeViewModel ethalonDiscs, DiscTreeViewModel currentDiscs);
	}
}
