using System.Collections.Generic;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer
{
	public interface IViewNavigator
	{
		void ShowRateDiscView(Disc disc);

		void ShowDiscPropertiesView(Disc disc);

		void ShowSongPropertiesView(IEnumerable<Song> songs);

		Task ShowEditDiscImageView(Disc disc);

		void ShowLibraryStatisticsView();
	}
}
