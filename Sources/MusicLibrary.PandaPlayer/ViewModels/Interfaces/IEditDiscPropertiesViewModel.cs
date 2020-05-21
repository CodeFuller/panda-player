using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IEditDiscPropertiesViewModel
	{
		string Title { get; set; }

		string TreeTitle { get; set; }

		string AlbumTitle { get; set; }

		void Load(DiscModel disc);

		Task Save(CancellationToken cancellationToken);
	}
}
