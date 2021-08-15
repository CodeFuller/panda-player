using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer
{
	public interface IViewNavigator
	{
		void ShowRatePlaylistSongsView(IEnumerable<SongModel> songs);

		string ShowCreateAdviseGroupView(string initialAdviseGroupName, IEnumerable<string> existingAdviseGroupNames);

		void ShowDiscPropertiesView(DiscModel disc);

		Task ShowSongPropertiesView(IEnumerable<SongModel> songs, CancellationToken cancellationToken);

		void ShowEditDiscImageView(DiscModel disc);

		Task ShowDiscAdderView(CancellationToken cancellationToken);

		Task ShowLibraryCheckerView(CancellationToken cancellationToken);

		Task ShowLibraryStatisticsView(CancellationToken cancellationToken);
	}
}
