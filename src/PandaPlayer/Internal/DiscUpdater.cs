using System;
using System.Collections.Generic;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Internal
{
	internal class DiscUpdater
	{
		private static readonly IReadOnlyDictionary<string, Action<DiscModel, DiscModel>> UpdateActions = new Dictionary<string, Action<DiscModel, DiscModel>>
		{
			{ nameof(DiscModel.Title), (src, dst) => dst.Title = src.Title },
			{ nameof(DiscModel.TreeTitle), (src, dst) => dst.TreeTitle = src.TreeTitle },
			{ nameof(DiscModel.AlbumTitle), (src, dst) => dst.AlbumTitle = src.AlbumTitle },
		};

		public static void UpdateDisc(DiscModel source, DiscModel target, string propertyName)
		{
			if (!UpdateActions.TryGetValue(propertyName, out var updateAction))
			{
				throw new InvalidOperationException($"Can not update unknown disc property '{propertyName}'");
			}

			updateAction(source, target);
		}
	}
}
