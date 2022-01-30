using System;

namespace PandaPlayer.Core.Models
{
	public class BasicModel
	{
		private ItemId id;

		public ItemId Id
		{
			get => id;
			set
			{
				if (id != null)
				{
					throw new InvalidOperationException("Can not overwrite model id");
				}

				id = value;
			}
		}
	}
}
