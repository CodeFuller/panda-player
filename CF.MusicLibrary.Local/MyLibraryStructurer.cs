using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.Local
{
	public class MyLibraryStructurer : ILibraryStructurer
	{
		public IEnumerable<Uri> GetAllPossibleArtistStorageUris(Artist artist)
		{
			return MyLocalLibraryNames.ArtistCategories
				.Select(c => BuildArtistUri(c, artist.Name));
		}

		public Uri GetArtistStorageUri(Artist artist)
		{
			var categories = artist.Songs
				.Select(s => new LocalLibraryDiscPath(s.Disc.Uri).Category)
				.Distinct()
				.ToList();

			if (categories.Count == 1 && MyLocalLibraryNames.IsArtistCategory(categories.Single()))
			{
				return BuildArtistUri(categories.Single(), artist.Name);
			}

			return null;
		}

		private static Uri BuildArtistUri(string category, string artist)
		{
			return ItemUriParts.Join(BuildCategoryUri(category), artist);
		}

		public Uri BuildArtistDiscUri(Uri artistUri, string discNamePart)
		{
			return ItemUriParts.Join(artistUri, discNamePart);
		}

		public Uri BuildSongUri(Uri discUri, string songFileName)
		{
			return ItemUriParts.Join(discUri, songFileName);
		}

		public Uri BuildUriForWorkshopStoragePath(string pathWithinStorage)
		{
			return ItemUriParts.Join(pathWithinStorage.Split(Path.DirectorySeparatorChar));
		}

		public Uri ReplaceDiscPartInUri(Uri discUri, string discPart)
		{
			var parts = (new ItemUriParts(discUri)).ToList();
			parts[parts.Count - 1] = discPart;
			return ItemUriParts.Join(parts);
		}

		private static Uri BuildCategoryUri(string category)
		{
			return ItemUriParts.Join(Enumerable.Repeat(category, 1));
		}
	}
}
