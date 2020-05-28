using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer
{
	public interface IViewNavigator
	{
		void ShowRatePlaylistSongsView(IEnumerable<SongModel> songs);

		void ShowDiscPropertiesView(DiscModel disc);

		Task ShowSongPropertiesView(IEnumerable<SongModel> songs, CancellationToken cancellationToken);

		void ShowEditDiscImageView(DiscModel disc);

		Task ShowLibraryStatisticsView(CancellationToken cancellationToken);
	}
}
