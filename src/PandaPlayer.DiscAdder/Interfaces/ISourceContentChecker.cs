using PandaPlayer.DiscAdder.ViewModels.Interfaces;

namespace PandaPlayer.DiscAdder.Interfaces
{
	internal interface ISourceContentChecker
	{
		void SetContentCorrectness(IReferenceContentViewModel referenceContent, IActualContentViewModel actualContent);
	}
}
