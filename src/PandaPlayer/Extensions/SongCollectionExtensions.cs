using System.Collections.Generic;
using System.Linq;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Extensions
{
	public static class SongCollectionExtensions
	{
		public static string GetDeleteComment(this IEnumerable<SongModel> songs, string valueForVariousDeleteComments)
		{
			var deleteComments = songs.Select(x => x.DeleteComment).Distinct().ToList();
			return deleteComments.Count switch
			{
				0 => null,
				1 => deleteComments.Single(),
				_ => valueForVariousDeleteComments,
			};
		}
	}
}
