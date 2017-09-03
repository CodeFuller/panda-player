using System;
using System.Collections.Generic;
using System.Linq;
using CF.Library.Core;
using CF.Library.Core.Facades;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.LibraryChecker.Registrators;
using CF.MusicLibrary.Tagger;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public class TagDataConsistencyChecker : ITagDataConsistencyChecker
	{
		private readonly IMusicStorage musicStorage;
		private readonly ISongTagger songTagger;
		private readonly IFileSystemFacade fileSystemFacade;
		private readonly ILibraryInconsistencyRegistrator inconsistencyRegistrator;

		public TagDataConsistencyChecker(IMusicStorage musicStorage, ISongTagger songTagger, IFileSystemFacade fileSystemFacade, ILibraryInconsistencyRegistrator inconsistencyRegistrator)
		{
			if (musicStorage == null)
			{
				throw new ArgumentNullException(nameof(musicStorage));
			}
			if (songTagger == null)
			{
				throw new ArgumentNullException(nameof(musicStorage));
			}
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}
			if (inconsistencyRegistrator == null)
			{
				throw new ArgumentNullException(nameof(inconsistencyRegistrator));
			}

			this.musicStorage = musicStorage;
			this.songTagger = songTagger;
			this.fileSystemFacade = fileSystemFacade;
			this.inconsistencyRegistrator = inconsistencyRegistrator;
		}

		public void CheckTagData(IEnumerable<Song> songs)
		{
			Application.Logger.WriteInfo("Checking tag data ...");

			foreach (var song in songs)
			{
				var songFileName = musicStorage.GetSongFile(song.Uri).FullName;
				var tagData = songTagger.GetTagData(songFileName);

				if (tagData.Artist != song.Artist?.Name)
				{
					inconsistencyRegistrator.RegisterInconsistency_BadTagData(Current($"Artist mismatch for {song.Uri + ":",-100} '{tagData.Artist}' != '{song.Artist?.Name}'"));
				}

				if (tagData.Album != song.Disc.AlbumTitle)
				{
					inconsistencyRegistrator.RegisterInconsistency_BadTagData(Current($"Album mismatch for {song.Uri + ":",-100} '{tagData.Album}' != '{song.Disc.AlbumTitle}'"));
				}
				if (AlbumTitleChecker.AlbumTitleIsSuspicious(tagData.Album))
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

				var tagTypes = songTagger.GetTagTypes(songFileName).ToList();
				if (!tagTypes.SequenceEqual(new[] { AudioTagType.Id3V1, AudioTagType.Id3V2 }))
				{
					inconsistencyRegistrator.RegisterInconsistency_BadTagData(Current($"Bad tag types for {song.Uri + ":",-100}: [{String.Join(", ", tagTypes)}]"));
				}
			}
		}

		public void UnifyTags(IEnumerable<Song> songs)
		{
			foreach (var song in songs)
			{
				var songFileName = musicStorage.GetSongFile(song.Uri).FullName;
				Application.Logger.WriteInfo(Current($"Unifying tag data for song '{songFileName}'..."));

				fileSystemFacade.ClearReadOnlyAttribute(songFileName);
				songTagger.FixTagData(songFileName);
				fileSystemFacade.SetReadOnlyAttribute(songFileName);
			}

			Application.Logger.WriteInfo("Tags unification has finished successfully");
		}
	}
}
