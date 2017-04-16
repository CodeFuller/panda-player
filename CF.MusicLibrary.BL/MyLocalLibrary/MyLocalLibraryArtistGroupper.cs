using System;
using System.IO;
using CF.MusicLibrary.BL.Objects;
using static System.FormattableString;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.BL.MyLocalLibrary
{
	public class MyLocalLibraryArtistGroupper : IDiscArtistGroupper, ICompilationDiscGroupper
	{
		private readonly string libraryRootDirectory;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="libraryRootDirectory">Path to root library directory (e.g. "d:\music")</param>
		public MyLocalLibraryArtistGroupper(string libraryRootDirectory)
		{
			if (libraryRootDirectory == null)
			{
				throw new ArgumentNullException(nameof(libraryRootDirectory));
			}

			this.libraryRootDirectory = libraryRootDirectory;
			this.libraryRootDirectory = this.libraryRootDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			this.libraryRootDirectory += Path.DirectorySeparatorChar;
		}

		public LibraryArtist GetDiscArtist(Disc disc)
		{
			DiscPathParts discPathParts = new DiscPathParts(disc, libraryRootDirectory);

			//	Any album under "Belarussian", "Foreign" or "Russian" directories is resolved as single-artist album
			//	This covers cases when Artist directory contains side-project albums (e.g. Гражданская Оборона/Егор Летов)

			if (MyLocalLibraryNames.IsArtistCategory(discPathParts.Category))
			{
				return GetArtistForCategory(discPathParts.Category, discPathParts.NestedDirectory);
			}

			//	Any album under "Soundtracks" or "Сборники" directories is resolved through ICompilationDiscGroupper
			if (discPathParts.Category == MyLocalLibraryNames.Soundtracks || discPathParts.Category == MyLocalLibraryNames.Collections)
			{
				return GetCompilationDiscArtist(disc);
			}

			throw new InvalidOperationException(Current($"Disc category '{discPathParts.Category}' is not recognized"));
		}

		public LibraryArtist GetCompilationDiscArtist(Disc disc)
		{
			DiscPathParts discPathParts = new DiscPathParts(disc, libraryRootDirectory);

			if (discPathParts.Category == MyLocalLibraryNames.Soundtracks)
			{
				return GetArtistForCategory(discPathParts.Category);
			}

			if (discPathParts.Category == MyLocalLibraryNames.Collections)
			{
				return GetCollectionArtist(discPathParts);
			}

			throw new InvalidOperationException(Current($"Disc category '{discPathParts.Category}' is not a compilation category"));
		}

		private static LibraryArtist GetCollectionArtist(DiscPathParts discPathParts)
		{
			if (MyLocalLibraryNames.IsRussianRockCollectionDirectory(discPathParts.NestedDirectory))
			{
				return GetArtistForCategory("Collections / Russian Rock");
			}

			if (MyLocalLibraryNames.IsBestCollectionDirectory(discPathParts.NestedDirectory))
			{
				return GetArtistForCategory("Collections / Best");
			}

			if (MyLocalLibraryNames.IsEurovisionDirectory(discPathParts.NestedDirectory))
			{
				return GetArtistForCategory("Collections / Eurovision");
			}

			return GetArtistForCategory("Collections / Different");
		}

		private static LibraryArtist GetArtistForCategory(string category, string artistName)
		{
			return new LibraryArtist(Invariant($"{category} / {artistName}"), artistName);
		}

		private static LibraryArtist GetArtistForCategory(string category)
		{
			return new LibraryArtist(category, category);
		}
	}
}
