﻿namespace PandaPlayer.Dal.LocalDb.Entities
{
	internal class ArtistEntity
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
