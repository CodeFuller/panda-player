using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CF.Library.Core;
using CF.MusicLibrary.Core;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.LibraryChecker.Registrators;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public class TagDataConsistencyChecker : ITagDataConsistencyChecker
	{
		private readonly IMusicLibrary musicLibrary;
		private readonly ILibraryInconsistencyRegistrator inconsistencyRegistrator;

		public TagDataConsistencyChecker(IMusicLibrary musicLibrary, ILibraryInconsistencyRegistrator inconsistencyRegistrator)
		{
			if (musicLibrary == null)
			{
				throw new ArgumentNullException(nameof(musicLibrary));
			}
			if (inconsistencyRegistrator == null)
			{
				throw new ArgumentNullException(nameof(inconsistencyRegistrator));
			}

			this.musicLibrary = musicLibrary;
			this.inconsistencyRegistrator = inconsistencyRegistrator;
		}

		public async Task CheckTagData(IEnumerable<Song> songs)
		{
			Application.Logger.WriteInfo("Checking tag data ...");

			foreach (var song in songs)
			{
				var tagData = await musicLibrary.GetSongTagData(song);

				if (tagData.Artist != song.Artist?.Name)
				{
					inconsistencyRegistrator.RegisterInconsistency_BadTagData(Current($"Artist mismatch for {song.Uri + ":",-100} '{tagData.Artist}' != '{song.Artist?.Name}'"));
				}

				if (tagData.Album != song.Disc.AlbumTitle)
				{
					inconsistencyRegistrator.RegisterInconsistency_BadTagData(Current($"Album mismatch for {song.Uri + ":",-100} '{tagData.Album}' != '{song.Disc.AlbumTitle}'"));
				}
				if (DiscTitleToAlbumMapper.AlbumTitleIsSuspicious(tagData.Album))
				{
					inconsistencyRegistrator.RegisterInconsistency_BadTagData(Current($"Album title looks suspicious for {song.Uri + ":",-100}: '{tagData.Album}'"));
				}

				if (tagData.Year != song.Year)
				{
					inconsistencyRegistrator.RegisterInconsistency_BadTagData(Current($"Year mismatch for {song.Uri + ":",-100} '{tagData.Year}' != '{song.Year}'"));
				}

				if (tagData.Genre != song.Genre?.Name)
				{
					inconsistencyRegistrator.RegisterInconsistency_BadTagData(Current($"Genre mismatch for {song.Uri + ":",-100} '{tagData.Genre}' != '{song.Genre?.Name}'"));
				}

				if (tagData.Track != song.TrackNumber)
				{
					inconsistencyRegistrator.RegisterInconsistency_BadTagData(Current($"Track # mismatch for {song.Uri + ":",-100} '{tagData.Track}' != '{song.TrackNumber}'"));
				}

				if (tagData.Title != song.Title)
				{
					inconsistencyRegistrator.RegisterInconsistency_BadTagData(Current($"Title mismatch for {song.Uri + ":",-100} '{tagData.Title}' != '{song.Title}'"));
				}

				var tagTypes = (await musicLibrary.GetSongTagTypes(song)).ToList();
				if (!tagTypes.SequenceEqual(new[] { SongTagType.Id3V1, SongTagType.Id3V2 }))
				{
					inconsistencyRegistrator.RegisterInconsistency_BadTagData(Current($"Bad tag types for {song.Uri + ":",-100}: [{String.Join(", ", tagTypes)}]"));
				}
			}
		}

		public async Task UnifyTags(IEnumerable<Song> songs)
		{
			foreach (var song in songs)
			{
				Application.Logger.WriteInfo(Current($"Unifying tag data for song '{song.Uri}'..."));
				await musicLibrary.FixSongTagData(song);
			}

			Application.Logger.WriteInfo("Tags unification has finished successfully");
		}
	}
}
