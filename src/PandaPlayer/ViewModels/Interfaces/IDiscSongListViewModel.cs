using System.Collections.Generic;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IDiscSongListViewModel : ISongListViewModel
	{
		IEnumerable<BasicMenuItem> ContextMenuItems { get; }
	}
}
