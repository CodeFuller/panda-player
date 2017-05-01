using System;
using CF.MusicLibrary.BL.Objects;
using static System.FormattableString;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.BL.MyLocalLibrary
{
	public class MyLocalLibraryArtistGroupper : IDiscArtistGroupper, ICompilationDiscGroupper
	{
		private readonly IStorageUrlBuilder storageUrlBuilder;

		public MyLocalLibraryArtistGroupper(IStorageUrlBuilder storageUrlBuilder)
		{
			if (storageUrlBuilder == null)
			{
				throw new ArgumentNullException(nameof(storageUrlBuilder));
			}

			this.storageUrlBuilder = storageUrlBuilder;
		}

		public LibraryArtist GetDiscArtist(Disc disc)
		{
			LocalLibraryDiscPath localLibraryDiscPath = new LocalLibraryDiscPath(disc);

			//	Any album under "Belarussian", "Foreign" or "Russian" directories is resolved as single-artist album
			//	This covers cases when Artist directory contains side-project albums (e.g. Гражданская Оборона/Егор Летов)

			if (MyLocalLibraryNames.IsArtistCategory(localLibraryDiscPath.Category))
			{
				return GetArtistForCategory(localLibraryDiscPath.Category, localLibraryDiscPath.NestedDirectory);
			}

			//	Any album under "Soundtracks" or "Сборники" directories is resolved through ICompilationDiscGroupper
			if (localLibraryDiscPath.Category == MyLocalLibraryNames.Soundtracks || localLibraryDiscPath.Category == MyLocalLibraryNames.Collections)
			{
				return GetCompilationDiscArtist(disc);
			}

			throw new InvalidOperationException(Current($"Disc category '{localLibraryDiscPath.Category}' is not recognized"));
		}

		public LibraryArtist GetCompilationDiscArtist(Disc disc)
		{
			LocalLibraryDiscPath localLibraryDiscPath = new LocalLibraryDiscPath(disc);

			if (localLibraryDiscPath.Category == MyLocalLibraryNames.Soundtracks)
			{
				return GetArtistForCategory(localLibraryDiscPath.Category);
			}

			if (localLibraryDiscPath.Category == MyLocalLibraryNames.Collections)
			{
				return GetCollectionArtist(localLibraryDiscPath);
			}

			throw new InvalidOperationException(Current($"Disc category '{localLibraryDiscPath.Category}' is not a compilation category"));
		}

		private static LibraryArtist GetCollectionArtist(LocalLibraryDiscPath localLibraryDiscPath)
		{
			if (MyLocalLibraryNames.IsRussianRockCollectionDirectory(localLibraryDiscPath.NestedDirectory))
			{
				return GetArtistForCategory("Collections / Russian Rock");
			}

			if (MyLocalLibraryNames.IsBestCollectionDirectory(localLibraryDiscPath.NestedDirectory))
			{
				return GetArtistForCategory("Collections / Best");
			}

			if (MyLocalLibraryNames.IsEurovisionDirectory(localLibraryDiscPath.NestedDirectory))
			{
				return GetArtistForCategory("Collections / Eurovision");
			}

			return GetArtistForCategory("Collections / Different");
		}

		private LibraryArtist GetArtistForCategory(string category, string artistName)
		{
			return new LibraryArtist(Invariant($"{category} / {artistName}"), artistName, storageUrlBuilder.BuildArtistStorageUrl(category, artistName));
		}

		private static LibraryArtist GetArtistForCategory(string category)
		{
			return new LibraryArtist(category, category);
		}
	}
}
