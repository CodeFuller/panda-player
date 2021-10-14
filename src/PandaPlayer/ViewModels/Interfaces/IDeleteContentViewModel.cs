using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IDeleteContentViewModel
	{
		string ConfirmationMessage { get; }

		string DeleteComment { get; set; }

		void LoadForSongs(IReadOnlyCollection<SongModel> songs);

		void LoadForDisc(DiscModel disc);

		Task Delete(CancellationToken cancellationToken);
	}
}
