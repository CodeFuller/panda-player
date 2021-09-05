using System.Collections.Generic;

namespace PandaPlayer.Core.Models
{
	public class AdviseSetModel
	{
		public ItemId Id { get; set; }

		public string Name { get; set; }

		private List<DiscModel> discs = new();

		public IReadOnlyCollection<DiscModel> Discs
		{
			get => discs;
			set => discs = new List<DiscModel>(value);
		}
	}
}
