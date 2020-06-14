using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.Diagnostic.Inconsistencies;
using MusicLibrary.Services.Diagnostic.Inconsistencies.TagsInconsistencies;
using MusicLibrary.Services.Diagnostic.Interfaces;
using MusicLibrary.Services.Interfaces.Dal;
using MusicLibrary.Services.Tagging;

namespace MusicLibrary.Services.Diagnostic.Checkers
{
	internal class TagsConsistencyChecker : ITagsConsistencyChecker
	{
		private readonly IStorageRepository storageRepository;

		public TagsConsistencyChecker(IStorageRepository storageRepository)
		{
			this.storageRepository = storageRepository ?? throw new ArgumentNullException(nameof(storageRepository));
		}

		public async Task CheckTagsConsistency(IEnumerable<SongModel> songs, Action<LibraryInconsistency> inconsistenciesHandler, CancellationToken cancellationToken)
		{
			foreach (var song in songs)
			{
				var tagData = await storageRepository.GetSongTagData(song, cancellationToken);

				if (tagData.Artist != song.Artist?.Name)
				{
					inconsistenciesHandler(new BadArtistTagInconsistency(song, tagData.Artist));
				}

				if (tagData.Album != song.Disc.AlbumTitle)
				{
					inconsistenciesHandler(new BadAlbumTagInconsistency(song, tagData.Album));
				}

				if (tagData.Year != song.Disc.Year)
				{
					inconsistenciesHandler(new BadYearTagInconsistency(song, tagData.Year));
				}

				if (tagData.Genre != song.Genre?.Name)
				{
					inconsistenciesHandler(new BadGenreTagInconsistency(song, tagData.Genre));
				}

				if (tagData.Track != song.TrackNumber)
				{
					inconsistenciesHandler(new BadTrackTagInconsistency(song, tagData.Track));
				}

				if (tagData.Title != song.Title)
				{
					inconsistenciesHandler(new BadTitleTagInconsistency(song, tagData.Title));
				}

				var tagTypes = await storageRepository.GetSongTagTypes(song, cancellationToken);
				var unexpectedTagTypes = tagTypes.Except(new[] { SongTagType.Id3V1, SongTagType.Id3V2 }).ToList();

				if (unexpectedTagTypes.Any())
				{
					inconsistenciesHandler(new UnexpectedTagTypesInconsistency(song, unexpectedTagTypes));
				}
			}
		}
	}
}
