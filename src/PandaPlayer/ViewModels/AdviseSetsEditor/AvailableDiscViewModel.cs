using System;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.AdviseSetsEditor
{
	public class AvailableDiscViewModel
	{
		public DiscModel Disc { get; }

		public string Title { get; }

		public AvailableDiscViewModel(DiscModel disc, string title)
		{
			Disc = disc ?? throw new ArgumentNullException(nameof(disc));
			Title = title ?? throw new ArgumentNullException(nameof(title));
		}
	}
}
