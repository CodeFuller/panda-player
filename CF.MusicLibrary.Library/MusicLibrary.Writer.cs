using System;
using System.Linq;
using System.Threading.Tasks;
using CF.MusicLibrary.Core;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;
using static CF.Library.Core.Application;

namespace CF.MusicLibrary.Library
{
	public partial class RepositoryAndStorageMusicLibrary : IMusicLibraryWriter, IMusicLibraryReader
	{
		public async Task AddSong(Song song, string songSourceFileName)
		{
			songTagger.SetTagData(songSourceFileName, FillSongTagData(song));
			song.Checksum = checksumCalculator.CalculateChecksumForFile(songSourceFileName);

			await libraryStorage.StoreSong(songSourceFileName, song);
			await libraryRepository.AddSong(song);
		}

		public async Task UpdateSong(Song song, UpdatedSongProperties updatedProperties)
		{
			if ((updatedProperties & SongTagData.TaggedProperties) != 0)
			{
				await UpdateSongTagData(song);
			}
			else
			{
				await libraryRepository.UpdateSong(song);
			}
		}

		public async Task ChangeSongUri(Song song, Uri newSongUri)
		{
			await libraryStorage.ChangeSongUri(song, newSongUri);
			song.Uri = newSongUri;
			await libraryRepository.UpdateSong(song);
		}

		public async Task DeleteSong(Song song, DateTime deleteTime)
		{
			await libraryStorage.DeleteSong(song);

			song.DeleteDate = deleteTime;
			await libraryRepository.UpdateSong(song);
		}

		public async Task UpdateDisc(Disc disc, UpdatedSongProperties updatedProperties)
		{
			if ((updatedProperties & SongTagData.TaggedProperties) != 0)
			{
				foreach (var song in disc.Songs)
				{
					await UpdateSongTagData(song);
				}
			}
			await libraryRepository.UpdateDisc(disc);
		}

		public async Task ChangeDiscUri(Disc disc, Uri newDiscUri)
		{
			await libraryStorage.ChangeDiscUri(disc, newDiscUri);
			disc.Uri = newDiscUri;
			await libraryRepository.UpdateDisc(disc);

			foreach (var song in disc.Songs)
			{
				song.Uri = libraryStructurer.ReplaceDiscPartInSongUri(newDiscUri, song.Uri);
				await libraryRepository.UpdateSong(song);
			}

			foreach (var image in disc.Images)
			{
				image.Uri = libraryStructurer.ReplaceDiscPartInImageUri(newDiscUri, image.Uri);
				await libraryRepository.UpdateDiscImage(image);
			}
		}

		public async Task DeleteDisc(Disc disc)
		{
			Logger.WriteInfo($"Deleting disc '{disc.Title}'");
			var deleteTime = DateTimeFacade.Now;
			foreach (var song in disc.Songs)
			{
				Logger.WriteInfo($"Deleting song '{song.Uri}'");
				await DeleteSong(song, deleteTime);
			}

			//	Deleting image from repository modifies disc.Images.
			//	That's why we preserve original images collection with .ToList().
			foreach (var image in disc.Images.ToList())
			{
				Logger.WriteInfo($"Deleting disc image '{image.Uri}'");
				await libraryStorage.DeleteDiscImage(image);
				await libraryRepository.DeleteDiscImage(image);
			}

			Logger.WriteInfo($"Disc '{disc.Title}' was deleted successfully");
		}

		public async Task SetDiscCoverImage(Disc disc, ImageInfo imageInfo)
		{
			if (disc.CoverImage != null)
			{
				await libraryStorage.DeleteDiscImage(disc.CoverImage);
				await libraryRepository.DeleteDiscImage(disc.CoverImage);
				disc.CoverImage = null;
			}

			var coverImage = new DiscImage
			{
				Uri = libraryStructurer.GetDiscCoverImageUri(disc.Uri, imageInfo),
				FileSize = (int)imageInfo.FileSize,
				Checksum = checksumCalculator.CalculateChecksumForFile(imageInfo.FileName),
				Disc = disc,
				ImageType = DiscImageType.Cover,
			};

			await libraryStorage.StoreDiscImage(imageInfo.FileName, coverImage);
			await libraryRepository.AddDiscImage(coverImage);
			disc.CoverImage = coverImage;
		}

		public async Task AddSongPlayback(Song song, DateTime playbackTime)
		{
			await libraryRepository.AddSongPlayback(song, playbackTime);
		}

		public async Task FixSongTagData(Song song)
		{
			await UpdateSongContent(song, songFileName => songTagger.FixTagData(songFileName));
		}

		private async Task UpdateSongTagData(Song song)
		{
			await UpdateSongContent(song, songFileName => songTagger.SetTagData(songFileName, FillSongTagData(song)));
		}

		private async Task UpdateSongContent(Song song, Action<string> updateAction)
		{
			var songFileName = await libraryStorage.GetSongFileForWriting(song);
			updateAction(songFileName);
			await UpdateSongChecksum(song, songFileName);

			await libraryStorage.UpdateSongContent(songFileName, song);
		}

		private async Task UpdateSongChecksum(Song song, string songFileName)
		{
			song.Checksum = checksumCalculator.CalculateChecksumForFile(songFileName);
			await libraryRepository.UpdateSong(song);
		}

		private static SongTagData FillSongTagData(Song song)
		{
			return new SongTagData
			{
				Artist = song.Artist?.Name,
				Album = song.Disc.AlbumTitle,
				Year = song.Year,
				Genre = song.Genre?.Name,
				Track = song.TrackNumber,
				Title = song.Title,
			};
		}
	}
}
