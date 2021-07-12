using System.ComponentModel;

namespace PandaPlayer.DiscAdder.ViewModels.Interfaces
{
	internal interface IPageViewModel : INotifyPropertyChanged
	{
		string Name { get; }

		bool DataIsReady { get; }
	}
}
