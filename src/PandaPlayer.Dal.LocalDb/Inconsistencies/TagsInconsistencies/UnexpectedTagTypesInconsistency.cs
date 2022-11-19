using System;
using System.Collections.Generic;
using System.Linq;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Tagging;

namespace PandaPlayer.Dal.LocalDb.Inconsistencies.TagsInconsistencies
{
	internal class UnexpectedTagTypesInconsistency : BasicTagInconsistency
	{
		private readonly IReadOnlyCollection<SongTagType> tagTypes;

		public override string Description => $"Unexpected tag types for '{SongDisplayTitle}': [{String.Join(", ", tagTypes)}]";

		public UnexpectedTagTypesInconsistency(SongModel song, IEnumerable<SongTagType> tagTypes)
			: base(song)
		{
			this.tagTypes = tagTypes?.ToList() ?? throw new ArgumentNullException(nameof(tagTypes));
		}
	}
}
