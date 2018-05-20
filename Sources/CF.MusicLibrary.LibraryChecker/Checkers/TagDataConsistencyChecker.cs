using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.LibraryChecker.Registrators;
using Microsoft.Extensions.Logging;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public class TagDataConsistencyChecker : ITagDataConsistencyChecker
	{
		private readonly IMusicLibrary musicLibrary;
		private readonly ILibraryInconsistencyRegistrator inconsistencyRegistrator;
		private readonly IDiscTitleToAlbumMapper discTitleToAlbumMapper;
		private readonly ICheckScope checkScope;
		private readonly ILogger<TagDataConsistencyChecker> logger;

		public TagDataConsistencyChecker(IMusicLibrary musicLibrary, ILibraryInconsistencyRegistrator inconsistencyRegistrator,
			IDiscTitleToAlbumMapper discTitleToAlbumMapper, ICheckScope checkScope, ILogger<TagDataConsistencyChecker> logger)
		{
			this.musicLibrary = musicLibrary ?? throw new ArgumentNullException(nameof(musicLibrary));
			this.inconsistencyRegistrator = inconsistencyRegistrator ?? throw new ArgumentNullException(nameof(inconsistencyRegistrator));
			this.discTitleToAlbumMapper = discTitleToAlbumMapper ?? throw new ArgumentNullException(nameof(discTitleToAlbumMapper));
			this.checkScope = checkScope ?? throw new ArgumentNullException(nameof(checkScope));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task CheckTagData(IEnumerable<Song> songs, CancellationToken cancellationToken)
		{
			logger.LogInformation("Checking tag data ...");

			foreach (var song in songs.Where(checkScope.Contains))
			{
				var tagData = await musicLibrary.GetSongTagData(song);

				if (tagData.Artist != song.Artist?.Name)
				{
					inconsistencyRegistrator.RegisterBadTagData(Current($"Artist mismatch for {song.Uri + ":", -100} '{tagData.Artist}' != '{song.Artist?.Name}'"));
				}

				if (tagData.Album != song.Disc.AlbumTitle)
				{
					inconsistencyRegistrator.RegisterBadTagData(Current($"Album mismatch for {song.Uri + ":", -100} '{tagData.Album}' != '{song.Disc.AlbumTitle}'"));
				}

				if (discTitleToAlbumMapper.AlbumTitleIsSuspicious(tagData.Album))
				{
					inconsistencyRegistrator.RegisterBadTagData(Current($"Album title looks suspicious for {song.Uri + ":", -100}: '{tagData.Album}'"));
				}

				if (tagData.Year != song.Year)
				{
					inconsistencyRegistrator.RegisterBadTagData(Current($"Year mismatch for {song.Uri + ":", -100} '{tagData.Year}' != '{song.Year}'"));
				}

				if (tagData.Genre != song.Genre?.Name)
				{
					inconsistencyRegistrator.RegisterBadTagData(Current($"Genre mismatch for {song.Uri + ":", -100} '{tagData.Genre}' != '{song.Genre?.Name}'"));
				}

				if (tagData.Track != song.TrackNumber)
				{
					inconsistencyRegistrator.RegisterBadTagData(Current($"Track # mismatch for {song.Uri + ":", -100} '{tagData.Track}' != '{song.TrackNumber}'"));
				}

				if (tagData.Title != song.Title)
				{
					inconsistencyRegistrator.RegisterBadTagData(Current($"Title mismatch for {song.Uri + ":", -100} '{tagData.Title}' != '{song.Title}'"));
				}

				var tagTypes = (await musicLibrary.GetSongTagTypes(song)).ToList();
				if (!tagTypes.SequenceEqual(new[] { SongTagType.Id3V1, SongTagType.Id3V2 }))
				{
					inconsistencyRegistrator.RegisterBadTagData(Current($"Bad tag types for {song.Uri + ":", -100}: [{String.Join(", ", tagTypes)}]"));
				}
			}
		}

		public async Task UnifyTags(IEnumerable<Song> songs, CancellationToken cancellationToken)
		{
			foreach (var song in songs.Where(checkScope.Contains))
			{
				logger.LogInformation(Current($"Unifying tag data for song '{song.Uri}'..."));
				await musicLibrary.FixSongTagData(song);
			}

			logger.LogInformation("Tags unification has finished successfully");
		}
	}
}
