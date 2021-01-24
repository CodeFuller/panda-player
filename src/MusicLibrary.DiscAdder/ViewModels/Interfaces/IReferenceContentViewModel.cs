using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace MusicLibrary.DiscAdder.ViewModels.Interfaces
{
	internal interface IReferenceContentViewModel : INotifyPropertyChanged
	{
		string Content { get; set; }

		Task LoadRawReferenceDiscsContent(CancellationToken cancellationToken);
	}
}
