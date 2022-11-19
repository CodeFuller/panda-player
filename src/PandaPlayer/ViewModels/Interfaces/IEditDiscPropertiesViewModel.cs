using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IEditDiscPropertiesViewModel
	{
		bool IsDeleted { get; }

		string Title { get; set; }

		string TreeTitle { get; set; }

		string AlbumTitle { get; set; }

		int? Year { get; set; }

		string DeleteComment { get; set; }

		void Load(DiscModel disc);

		Task Save(CancellationToken cancellationToken);
	}
}
