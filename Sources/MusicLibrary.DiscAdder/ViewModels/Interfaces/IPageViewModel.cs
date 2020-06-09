using System.ComponentModel;

namespace MusicLibrary.DiscAdder.ViewModels.Interfaces
{
	internal interface IPageViewModel : INotifyPropertyChanged
	{
		string Name { get; }

		bool DataIsReady { get; }
	}
}
