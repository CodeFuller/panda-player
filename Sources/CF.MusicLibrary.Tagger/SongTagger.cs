using System;
using System.Collections.Generic;
using System.IO;
using CF.MusicLibrary.Core.Media;
using TagLib;

namespace CF.MusicLibrary.Tagger
{
	public class SongTagger : ISongTagger
	{
		static SongTagger()
		{
			// Avoiding default usage of numeric genre code.
			// Otherwise we can't reach exact genre requested by the user.
			// For example 'Alternative Rock' will result in 'AlternRock' for numeric genres mode.
			TagLib.Id3v2.Tag.UseNumericGenres = false;
		}

		public void SetTagData(string songFileName, SongTagData tagData)
		{
			CheckSourceFile(songFileName);

			using (TagLib.File file = TagLib.File.Create(songFileName))
			{
				SetTagData(file, tagData);
			}
		}

		private static void SetTagData(TagLib.File file, SongTagData tagData)
		{
			file.RemoveTags(TagTypes.Id3v1);
			file.RemoveTags(TagTypes.Id3v2);
			file.RemoveTags(TagTypes.Ape);

			// We set only Id3v2 tag.
			// Id3v1 stores genres as index from predefined genre list.
			// This list is pretty limited and doesn't contain frequently used tags like 'Symphonic Metal' or 'Nu metal'.
			// The list of available genres for Id3v2 tag could be obtained from TagLib.Genres.Audio.
			var tag = file.GetTag(TagTypes.Id3v2, true);
			FillTag(tag, tagData);
			file.Save();
		}

		public SongTagData GetTagData(string songFileName)
		{
			using (TagLib.File file = TagLib.File.Create(songFileName))
			{
				var tag = file.GetTag(TagTypes.Id3v2);
				return ExtractTagData(tag);
			}
		}

		public IEnumerable<SongTagType> GetTagTypes(string songFileName)
		{
			Dictionary<TagTypes, SongTagType> tagTypesMap = new Dictionary<TagTypes, SongTagType>
			{
				{ TagTypes.Xiph, SongTagType.Xiph },
				{ TagTypes.Id3v1, SongTagType.Id3V1 },
				{ TagTypes.Id3v2, SongTagType.Id3V2 },
				{ TagTypes.Ape, SongTagType.Ape },
				{ TagTypes.Apple, SongTagType.Apple },
				{ TagTypes.Asf, SongTagType.Asf },
				{ TagTypes.RiffInfo, SongTagType.Riff },
				{ TagTypes.FlacMetadata, SongTagType.Flac },
				{ TagTypes.AudibleMetadata, SongTagType.Audible },
				{ TagTypes.XMP, SongTagType.Xmp },
			};

			using (TagLib.File file = TagLib.File.Create(songFileName))
			{
				var tagTypes = file.TagTypes;
				foreach (var checkedTagType in tagTypesMap)
				{
					if ((tagTypes & checkedTagType.Key) != 0)
					{
						yield return checkedTagType.Value;
						tagTypes &= ~checkedTagType.Key;
					}
				}

				if (tagTypes != 0)
				{
					yield return SongTagType.Unknown;
				}
			}
		}

		public void FixTagData(string songFileName)
		{
			CheckSourceFile(songFileName);

			using (TagLib.File file = TagLib.File.Create(songFileName))
			{
				var tagData = ExtractTagData(file.GetTag(TagTypes.Id3v2));
				SetTagData(file, tagData);
			}
		}

		private static void CheckSourceFile(string songFileName)
		{
			string extension = Path.GetExtension(songFileName);
			if (extension != ".mp3")
			{
				throw new InvalidOperationException("Only setting of mp3 tags is supported");
			}
		}

		private static SongTagData ExtractTagData(Tag tag)
		{
			return new SongTagData
			{
				Artist = FixSlashInArtistName(tag),
				Album = tag.Album,
				Year = tag.Year == 0 ? null : (int?)tag.Year,
				Genre = FixSlashInGenre(tag),
				Track = tag.Track == 0 ? null : (int?)tag.Track,
				Title = tag.Title,
			};
		}

		private static void FillTag(Tag tag, SongTagData tagData)
		{
			if (!String.IsNullOrEmpty(tagData.Artist))
			{
				tag.Performers = new[] { tagData.Artist };
			}

			if (!String.IsNullOrEmpty(tagData.Album))
			{
				tag.Album = tagData.Album;
			}

			if (tagData.Year.HasValue)
			{
				tag.Year = (uint)tagData.Year;
			}

			if (!String.IsNullOrEmpty(tagData.Genre))
			{
				tag.Genres = new[] { tagData.Genre };
			}

			if (tagData.Track.HasValue)
			{
				tag.Track = (uint)tagData.Track;
			}

			tag.Title = tagData.Title;
		}

		private static string FixSlashInArtistName(Tag tag)
		{
			return tag.Performers.Length > 1 ? String.Join("/", tag.Performers) : tag.FirstPerformer;
		}

		private static string FixSlashInGenre(Tag tag)
		{
			return tag.Genres.Length > 1 ? String.Join("/", tag.Genres) : tag.FirstGenre;
		}
	}
}
