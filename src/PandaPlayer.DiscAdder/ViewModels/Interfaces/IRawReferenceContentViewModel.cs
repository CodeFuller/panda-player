using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace PandaPlayer.DiscAdder.ViewModels.Interfaces
{
	internal interface IRawReferenceContentViewModel : INotifyPropertyChanged
	{
		string Content { get; set; }

		Task LoadContent(CancellationToken cancellationToken);

		Task ClearContent(CancellationToken cancellationToken);
	}
}
