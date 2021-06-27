using System.Collections.Generic;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Adviser.Grouping
{
	internal class DiscGroup
	{
		private readonly List<DiscModel> discs = new();

		public string Id { get; }

		public IReadOnlyCollection<DiscModel> Discs => discs;

		public DiscGroup(string id)
		{
			Id = id;
		}

		public void AddDisc(DiscModel disc)
		{
			discs.Add(disc);
		}
	}
}
