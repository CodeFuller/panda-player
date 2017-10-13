using System;
using System.Linq;
using CF.MusicLibrary.Core;
using CF.MusicLibrary.Core.Interfaces;

namespace CF.MusicLibrary.Local
{
	public class MyLibraryStructurer : ILibraryStructurer
	{
		public Uri BuildSongUri(Uri discUri, string songFileName)
		{
			return ItemUriParts.Join(discUri, songFileName);
		}

		public Uri ReplaceDiscPartInUri(Uri discUri, string discPart)
		{
			var parts = (new ItemUriParts(discUri)).ToList();
			parts[parts.Count - 1] = discPart;
			return ItemUriParts.Join(parts);
		}

		public Uri ReplaceDiscPartInSongUri(Uri newDiscUri, Uri songUri)
		{
			return BuildSongUri(newDiscUri, GetSongFileName(songUri));
		}

		public string GetDiscFolderName(Uri discUri)
		{
			var parts = (new ItemUriParts(discUri)).ToList();
			return parts[parts.Count - 1];
		}

		public string GetSongFileName(Uri songUri)
		{
			var parts = (new ItemUriParts(songUri)).ToList();
			return parts[parts.Count - 1];
		}
	}
}
