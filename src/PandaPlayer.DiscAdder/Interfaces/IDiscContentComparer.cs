using PandaPlayer.DiscAdder.ViewModels.SourceContent;

namespace PandaPlayer.DiscAdder.Interfaces
{
	internal interface IDiscContentComparer
	{
		void SetDiscsCorrectness(DiscTreeViewModel referenceDiscs, DiscTreeViewModel currentDiscs);
	}
}
