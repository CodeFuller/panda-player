using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using CF.MusicLibrary.DiscPreprocessor.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Options;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.DiscPreprocessor.ViewModels
{
	public class AddToLibraryViewModel : ViewModelBase, IAddToLibraryViewModel
	{
		private readonly IMusicLibrary musicLibrary;
		private readonly ISongMediaInfoProvider songMediaInfoProvider;
		private readonly IWorkshopMusicStorage workshopMusicStorage;

		private readonly bool deleteSourceContent;

		private List<AddedSong> addedSongs;
		private List<AddedDiscImage> addedDiscImages;

		public string Name => "Add To Library";

		private bool dataIsReady;
		public bool DataIsReady
		{
			get => dataIsReady;
			set => Set(ref dataIsReady, value);
		}

		public ICommand AddToLibraryCommand { get; }

		private int currProgress;
		public int CurrProgress
		{
			get => currProgress;
			set => Set(ref currProgress, value);
		}

		private int progressSize;
		public int ProgressSize
		{
			get => progressSize;
			set => Set(ref progressSize, value);
		}

		private string progressMessages;
		public string ProgressMessages
		{
			get => progressMessages;
			set => Set(ref progressMessages, value);
		}

		public AddToLibraryViewModel(IMusicLibrary musicLibrary, ISongMediaInfoProvider songMediaInfoProvider,
			IWorkshopMusicStorage workshopMusicStorage, IOptions<DiscPreprocessorSettings> options)
		{
			this.musicLibrary = musicLibrary ?? throw new ArgumentNullException(nameof(musicLibrary));
			this.songMediaInfoProvider = songMediaInfoProvider ?? throw new ArgumentNullException(nameof(songMediaInfoProvider));
			this.workshopMusicStorage = workshopMusicStorage ?? throw new ArgumentNullException(nameof(workshopMusicStorage));
			this.deleteSourceContent = options?.Value?.DeleteSourceContentAfterAdding ?? throw new ArgumentNullException(nameof(options));

			AddToLibraryCommand = new AsyncRelayCommand(AddContentToLibrary);
		}

		public void SetSongs(IEnumerable<AddedSong> songs)
		{
			addedSongs = songs.ToList();
		}

		public void SetDiscsImages(IEnumerable<AddedDiscImage> images)
		{
			addedDiscImages = images.ToList();
		}

		public async Task AddContentToLibrary()
		{
			if (addedSongs == null)
			{
				throw new InvalidOperationException("No songs for adding to the library");
			}

			DataIsReady = false;

			CurrProgress = 0;
			ProgressSize = 0;
			ProgressSize += await FillSongsMediaData(true);
			ProgressSize += await AddSongsToLibrary(true);
			await FillSongsMediaData(false);
			await AddSongsToLibrary(false);

			if (deleteSourceContent)
			{
				ProgressMessages += Current($"Deleting source content...\n");
				workshopMusicStorage.DeleteSourceContent(addedSongs.Select(s => s.SourceFileName).Concat(addedDiscImages.Select(im => im.ImageInfo.FileName)));
				ProgressMessages += Current($"Source content was deleted successfully\n");
			}

			DataIsReady = true;
		}

		private async Task<int> FillSongsMediaData(bool onlyCountProgressSize)
		{
			const int progressIncrement = 1;
			int taskProgressSize = addedSongs.Count * progressIncrement;

			if (onlyCountProgressSize)
			{
				return taskProgressSize;
			}

			foreach (var addedSong in addedSongs)
			{
				ProgressMessages += Current($"Getting media info for '{addedSong.SourceFileName}'...\n");
				var mediaInfo = await songMediaInfoProvider.GetSongMediaInfo(addedSong.SourceFileName);
				addedSong.Song.FileSize = mediaInfo.Size;
				addedSong.Song.Bitrate = mediaInfo.Bitrate;
				addedSong.Song.Duration = mediaInfo.Duration;

				CurrProgress += progressIncrement;
			}

			return taskProgressSize;
		}

		private async Task<int> AddSongsToLibrary(bool onlyCountProgressSize)
		{
			const int progressIncrement = 5;
			int taskProgressSize = 0;

			foreach (AddedSong addedSong in addedSongs)
			{
				if (!onlyCountProgressSize)
				{
					ProgressMessages += Current($"Adding to library '{addedSong.SourceFileName}'...\n");
					await musicLibrary.AddSong(addedSong.Song, addedSong.SourceFileName);
					CurrProgress += progressIncrement;
				}

				taskProgressSize += progressIncrement;
			}

			if (addedDiscImages != null)
			{
				foreach (AddedDiscImage image in addedDiscImages)
				{
					if (!onlyCountProgressSize)
					{
						ProgressMessages += Current($"Adding disc image '{image.ImageInfo.FileName}'...\n");
						await musicLibrary.SetDiscCoverImage(image.Disc, image.ImageInfo);
						CurrProgress += progressIncrement;
					}

					taskProgressSize += progressIncrement;
				}
			}

			return taskProgressSize;
		}
	}
}
