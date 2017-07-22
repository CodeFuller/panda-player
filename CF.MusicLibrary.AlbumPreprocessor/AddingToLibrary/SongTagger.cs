﻿using System;
using System.IO;
using System.Threading.Tasks;
using TagLib;

namespace CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary
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

		public async Task SetTagData(TaggedSongData tagData)
		{
			string extension = Path.GetExtension(tagData.SourceFileName);
			if (extension != ".mp3")
			{
				throw new InvalidOperationException("Only setting of mp3 tags is supported");
			}

			await Task.Run(() =>
			{
				TagLib.File f = TagLib.File.Create(tagData.SourceFileName);
				f.RemoveTags(TagTypes.Id3v1);
				f.RemoveTags(TagTypes.Id3v2);

				//	We set only Id3v2 tag.
				//	Id3v1 stores genres as index from predefined genre list.
				//	This list is pretty limited and doesn't contain frequently used tags like 'Symphonic Metal' or 'Nu metal'.
				//	The list of available genres for Id3v2 tag could be obtained from TagLib.Genres.Audio.

				var tag = f.GetTag(TagTypes.Id3v2, true);
				FillTag(tag, tagData);
				f.Save();
			});
		}

		static void FillTag(Tag tag, TaggedSongData tagData)
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
	}
}