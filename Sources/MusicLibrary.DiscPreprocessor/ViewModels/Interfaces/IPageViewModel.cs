using System.ComponentModel;

namespace MusicLibrary.DiscPreprocessor.ViewModels.Interfaces
{
	public interface IPageViewModel : INotifyPropertyChanged
	{
		string Name { get; }

		bool DataIsReady { get; }
	}
}
