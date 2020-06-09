using System.ComponentModel;

namespace MusicLibrary.DiscAdder.ViewModels.Interfaces
{
	public interface IPageViewModel : INotifyPropertyChanged
	{
		string Name { get; }

		bool DataIsReady { get; }
	}
}
