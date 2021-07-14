using System.ComponentModel;

namespace PandaPlayer.DiscAdder.ViewModels.Interfaces
{
	public interface IPageViewModel : INotifyPropertyChanged
	{
		string Name { get; }

		bool DataIsReady { get; }
	}
}
