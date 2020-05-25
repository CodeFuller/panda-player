using System;
using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Entities;
using MusicLibrary.Dal.LocalDb.Internal;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class FolderEntityExtensions
	{
		public static ShallowFolderModel ToModel(this FolderEntity folder, Uri folderUri)
		{
			return new ShallowFolderModel
			{
				Id = folderUri.ToItemId(),
				Name = new ItemUriParts(folderUri).Last(),
			};
		}
	}
}
