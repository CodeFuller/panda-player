using System.ComponentModel;

namespace CF.MusicLibrary.DiscPreprocessor.ViewModels.Interfaces
{
	public interface IPageViewModel : INotifyPropertyChanged
	{
		string Name { get; }

		bool DataIsReady { get; }
	}
}
