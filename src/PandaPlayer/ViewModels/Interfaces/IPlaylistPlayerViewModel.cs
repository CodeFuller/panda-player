using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PandaPlayer.ViewModels.Player;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IPlaylistPlayerViewModel
	{
		ISongPlayerViewModel SongPlayerViewModel { get; }

		ICommand ReversePlayingCommand { get; }

		Task ReversePlaying(CancellationToken cancellationToken);
	}
}
