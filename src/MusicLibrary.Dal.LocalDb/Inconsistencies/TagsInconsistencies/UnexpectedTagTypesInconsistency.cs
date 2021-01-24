using System;
using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.Tagging;

namespace MusicLibrary.Dal.LocalDb.Inconsistencies.TagsInconsistencies
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
