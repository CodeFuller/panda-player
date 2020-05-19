using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer
{
	public interface IViewNavigator
	{
		void ShowRateDiscView(Disc disc);

		void ShowDiscPropertiesView(Disc disc);

		Task ShowSongPropertiesView(IEnumerable<Song> songs, CancellationToken cancellationToken);

		Task ShowEditDiscImageView(Disc disc);

		void ShowLibraryStatisticsView();
	}
}
