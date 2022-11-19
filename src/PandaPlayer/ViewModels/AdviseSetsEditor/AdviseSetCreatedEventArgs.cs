using System;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.AdviseSetsEditor
{
	public class AdviseSetCreatedEventArgs : EventArgs
	{
		public AdviseSetModel AdviseSet { get; }

		public AdviseSetCreatedEventArgs(AdviseSetModel adviseSet)
		{
			AdviseSet = adviseSet ?? throw new ArgumentNullException(nameof(adviseSet));
		}
	}
}
