using System.Collections.Generic;

namespace PandaPlayer.Dal.LocalDb.Entities
{
	internal class ArtistEntity
	{
		public int Id { get; set; }

		public string Name { get; set; }

		// TODO: Remove this collection after refactoring loading of DiscLibrary.
		public IReadOnlyCollection<SongEntity> Songs { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
