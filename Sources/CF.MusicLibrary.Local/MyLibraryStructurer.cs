﻿using System;
using System.IO;
using System.Linq;
using CF.MusicLibrary.Core;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects.Images;

namespace CF.MusicLibrary.Local
{
	public class MyLibraryStructurer : ILibraryStructurer
	{
		private const string DiscCoverImageFileName = "cover";

		public Uri BuildSongUri(Uri discUri, string songFileName)
		{
			return BuildDiscFileUri(discUri, songFileName);
		}

		public Uri ReplaceDiscPartInUri(Uri discUri, string discPart)
		{
			var parts = (new ItemUriParts(discUri)).ToList();
			parts[parts.Count - 1] = discPart;
			return ItemUriParts.Join(parts);
		}

		public Uri ReplaceDiscPartInSongUri(Uri newDiscUri, Uri songUri)
		{
			return BuildDiscFileUri(newDiscUri, GetFileNameFromUri(songUri));
		}

		public Uri ReplaceDiscPartInImageUri(Uri newDiscUri, Uri imageUri)
		{
			return BuildDiscFileUri(newDiscUri, GetFileNameFromUri(imageUri));
		}

		public string GetDiscFolderName(Uri discUri)
		{
			var parts = (new ItemUriParts(discUri)).ToList();
			return parts[parts.Count - 1];
		}

		public string GetFileNameFromUri(Uri songUri)
		{
			var parts = (new ItemUriParts(songUri)).ToList();
			return parts[parts.Count - 1];
		}

		public Uri GetDiscCoverImageUri(Uri discUri, ImageInfo imageInfo)
		{
			return ItemUriParts.Join(discUri, Path.ChangeExtension(DiscCoverImageFileName, imageInfo.GetFileNameExtension()));
		}

		private static Uri BuildDiscFileUri(Uri discUri, string fileName)
		{
			return ItemUriParts.Join(discUri, fileName);
		}
	}
}