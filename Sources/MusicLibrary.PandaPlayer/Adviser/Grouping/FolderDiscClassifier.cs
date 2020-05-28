using System.Collections.Generic;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Interfaces;

namespace MusicLibrary.PandaPlayer.Adviser.Grouping
{
	internal class FolderDiscClassifier : IDiscClassifier
	{
		public IEnumerable<DiscGroup> GroupLibraryDiscs(IEnumerable<DiscModel> discs)
		{
			var groups = new Dictionary<string, DiscGroup>();

			// All discs from one folder are assigned to one group.
			foreach (var disc in discs)
			{
				var groupId = disc.Folder.Id.Value;

				if (!groups.TryGetValue(groupId, out var group))
				{
					group = new DiscGroup(groupId);
					groups.Add(groupId, group);
				}

				group.AddDisc(disc);
			}

			return groups.Values;
		}
	}
}
