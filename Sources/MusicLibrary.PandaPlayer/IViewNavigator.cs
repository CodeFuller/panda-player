using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer
{
	public interface IViewNavigator
	{
		void ShowRatePlaylistSongsView(IEnumerable<SongModel> songs);

		void ShowDiscPropertiesView(DiscModel disc);

		Task ShowSongPropertiesView(IEnumerable<SongModel> songs, CancellationToken cancellationToken);

		Task ShowEditDiscImageView(DiscModel disc);

		Task ShowLibraryStatisticsView(CancellationToken cancellationToken);
	}
}
