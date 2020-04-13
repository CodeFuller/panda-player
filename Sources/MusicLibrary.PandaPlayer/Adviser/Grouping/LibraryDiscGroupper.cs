using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MusicLibrary.Core.Objects;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.PandaPlayer.Adviser.Grouping
{
	public class LibraryDiscGroupper : IDiscGroupper
	{
		private readonly GroupingSettings groupingSettings;

		public LibraryDiscGroupper(IOptions<GroupingSettings> options)
		{
			this.groupingSettings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public IEnumerable<DiscGroup> GroupLibraryDiscs(DiscLibrary library)
		{
			var groups = new Dictionary<string, DiscGroup>();
			foreach (var disc in library.AllDiscs)
			{
				bool matchedGroup = false;
				foreach (var grouping in groupingSettings)
				{
					if (grouping.Matches(disc.Uri, out var groupId))
					{
						if (!groups.TryGetValue(groupId, out var group))
						{
							group = new DiscGroup(groupId);
							groups.Add(groupId, group);
						}

						group.Discs.Add(disc);
						matchedGroup = true;
						break;
					}
				}

				if (!matchedGroup)
				{
					throw new InvalidOperationException(Current($"Could not group disc '{disc.Uri}'"));
				}
			}

			return groups.Values;
		}
	}
}
