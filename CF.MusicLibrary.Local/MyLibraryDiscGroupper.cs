using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Universal;
using CF.MusicLibrary.Universal.Interfaces;
using static System.FormattableString;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.Local
{
	public class MyLibraryDiscGroupper : IDiscGroupper
	{
		public IEnumerable<DiscGroup> GroupLibraryDiscs(DiscLibrary library)
		{
			List<DiscGroup> groups = new List<DiscGroup>();
			foreach (var disc in library.AllDiscs)
			{
				var discGroup = GetGroupForDisc(disc);
				var group = groups.SingleOrDefault(g => g.Id == discGroup.Id);
				if (group == null)
				{
					groups.Add(discGroup);
					group = discGroup;
				}

				group.Discs.Add(disc);
			}

			return groups;
		}

		private static DiscGroup GetGroupForDisc(Disc disc)
		{
			LocalLibraryDiscPath localLibraryDiscPath = new LocalLibraryDiscPath(disc);

			//	Discs from "Belarussian", "Foreign" or "Russian" categories are groupped by 2nd folder.
			//	This covers cases when Artist directory contains side-project discs (e.g. Гражданская Оборона/Егор Летов).
			if (MyLocalLibraryNames.IsArtistCategory(localLibraryDiscPath.Category))
			{
				string artistName = localLibraryDiscPath.NestedDirectory;
				return new DiscGroup(Invariant($"{localLibraryDiscPath.Category} / {artistName}"), artistName);
			}

			//	All discs from "Soundtracks" category fall into one group.
			if (localLibraryDiscPath.Category == MyLocalLibraryNames.Soundtracks)
			{
				return new DiscGroup(localLibraryDiscPath.Category);
			}

			//	Disc from "Collections" category are groupped by compilation type
			if (localLibraryDiscPath.Category == MyLocalLibraryNames.Collections)
			{
				return GetCompilationDiscGroup(localLibraryDiscPath.NestedDirectory);
			}

			throw new InvalidOperationException(Current($"Disc category '{localLibraryDiscPath.Category}' is not recognized"));
		}

		private static DiscGroup GetCompilationDiscGroup(string nestedDirectory)
		{
			if (MyLocalLibraryNames.IsRussianRockCollectionDirectory(nestedDirectory))
			{
				return new DiscGroup("Collections / Russian Rock");
			}

			if (MyLocalLibraryNames.IsBestCollectionDirectory(nestedDirectory))
			{
				return new DiscGroup("Collections / Best");
			}

			if (MyLocalLibraryNames.IsEurovisionDirectory(nestedDirectory))
			{
				return new DiscGroup("Collections / Eurovision");
			}

			return new DiscGroup("Collections / Different");
		}
	}
}
