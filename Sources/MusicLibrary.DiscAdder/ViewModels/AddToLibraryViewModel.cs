﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Options;
using MusicLibrary.Core.Models;
using MusicLibrary.DiscAdder.AddingToLibrary;
using MusicLibrary.DiscAdder.MusicStorage;
using MusicLibrary.DiscAdder.ViewModels.Interfaces;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Services.Media;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.DiscAdder.ViewModels
{
	internal class AddToLibraryViewModel : ViewModelBase, IAddToLibraryViewModel
	{
		private readonly ISongMediaInfoProvider songMediaInfoProvider;
		private readonly IWorkshopMusicStorage workshopMusicStorage;

		private readonly IFoldersService foldersService;
		private readonly IDiscsService discService;
		private readonly ISongsService songService;
		private readonly IArtistsService artistService;

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

		private int currentProgress;

		public int CurrentProgress
		{
			get => currentProgress;
			set => Set(ref currentProgress, value);
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

		public AddToLibraryViewModel(ISongMediaInfoProvider songMediaInfoProvider, IWorkshopMusicStorage workshopMusicStorage,
			IFoldersService foldersService, IDiscsService discService, ISongsService songService, IArtistsService artistService, IOptions<DiscAdderSettings> options)
		{
			this.songMediaInfoProvider = songMediaInfoProvider ?? throw new ArgumentNullException(nameof(songMediaInfoProvider));
			this.workshopMusicStorage = workshopMusicStorage ?? throw new ArgumentNullException(nameof(workshopMusicStorage));
			this.foldersService = foldersService ?? throw new ArgumentNullException(nameof(foldersService));
			this.discService = discService ?? throw new ArgumentNullException(nameof(discService));
			this.songService = songService ?? throw new ArgumentNullException(nameof(songService));
			this.artistService = artistService ?? throw new ArgumentNullException(nameof(artistService));
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

			CurrentProgress = 0;
			ProgressSize = 0;
			ProgressSize += await FillSongsMediaData(true);
			ProgressSize += await AddSongsToLibrary(true, CancellationToken.None);
			await FillSongsMediaData(false);
			await AddSongsToLibrary(false, CancellationToken.None);

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
			var taskProgressSize = addedSongs.Count * progressIncrement;

			if (onlyCountProgressSize)
			{
				return taskProgressSize;
			}

			foreach (var addedSong in addedSongs)
			{
				ProgressMessages += Current($"Getting media info for '{addedSong.SourceFileName}'...\n");
				var mediaInfo = await songMediaInfoProvider.GetSongMediaInfo(addedSong.SourceFileName);

				addedSong.Song.BitRate = mediaInfo.Bitrate;
				addedSong.Song.Duration = mediaInfo.Duration;

				CurrentProgress += progressIncrement;
			}

			return taskProgressSize;
		}

		// TODO: Refactor to smaller methods
		private async Task<int> AddSongsToLibrary(bool onlyCountProgressSize, CancellationToken cancellationToken)
		{
			const int progressIncrement = 5;
			var taskProgressSize = 0;

			foreach (var addedSong in addedSongs)
			{
				if (!onlyCountProgressSize)
				{
					var song = addedSong.Song;

					if (song.Artist != null && song.Artist.Id == null)
					{
						await CreateArtist(song.Artist, cancellationToken);
					}

					var songDisc = song.Disc;
					if (songDisc.Id == null)
					{
						await CreateDisc(songDisc, addedSong.DiscFolderPath, cancellationToken);
					}

					ProgressMessages += Current($"Adding song '{addedSong.SourceFileName}'...\n");

					using var songContent = File.OpenRead(addedSong.SourceFileName);
					await songService.CreateSong(song, songContent, cancellationToken);

					CurrentProgress += progressIncrement;
				}

				taskProgressSize += progressIncrement;
			}

			if (addedDiscImages != null)
			{
				foreach (var image in addedDiscImages)
				{
					if (!onlyCountProgressSize)
					{
						ProgressMessages += Current($"Adding disc image '{image.ImageInfo.FileName}'...\n");

						var discImage = new DiscImageModel
						{
							Disc = image.Disc,
							TreeTitle = Path.GetFileName(image.ImageInfo.FileName),
							ImageType = DiscImageType.Cover,
						};

						using var imageContent = File.OpenRead(image.ImageInfo.FileName);
						await discService.SetDiscCoverImage(discImage, imageContent, cancellationToken);

						CurrentProgress += progressIncrement;
					}

					taskProgressSize += progressIncrement;
				}
			}

			return taskProgressSize;
		}

		private async Task CreateArtist(ArtistModel artist, CancellationToken cancellationToken)
		{
			ProgressMessages += Current($"Creating artist '{artist.Name}' ...\n");

			await artistService.CreateArtist(artist, cancellationToken);
		}

		private async Task<ShallowFolderModel> CreateFolder(IReadOnlyCollection<string> discFolderPath, CancellationToken cancellationToken)
		{
			const char pathSeparator = '/';

			var currentFolder = await foldersService.GetRootFolder(cancellationToken);

			var currentFolderFullPath = String.Empty;

			foreach (var currentSubfolderName in discFolderPath)
			{
				currentFolderFullPath += $"{pathSeparator}{currentSubfolderName}";

				var currentSubfolder = currentFolder.Subfolders.SingleOrDefault(sf => String.Equals(sf.Name, currentSubfolderName, StringComparison.Ordinal));
				if (currentSubfolder == null)
				{
					ProgressMessages += Current($"Creating folder '{currentFolderFullPath}' ...\n");

					currentSubfolder = new ShallowFolderModel
					{
						ParentFolderId = currentFolder.Id,
						Name = currentSubfolderName,
					};

					await foldersService.CreateFolder(currentSubfolder, cancellationToken);
				}

				currentFolder = await foldersService.GetFolder(currentSubfolder.Id, cancellationToken);
			}

			return currentFolder;
		}

		private async Task CreateDisc(DiscModel disc, IReadOnlyCollection<string> discFolderPath, CancellationToken cancellationToken)
		{
			if (disc.Folder == null)
			{
				disc.Folder = await CreateFolder(discFolderPath, cancellationToken);
			}

			ProgressMessages += Current($"Creating disc '{disc.TreeTitle}' ...\n");

			await discService.CreateDisc(disc, cancellationToken);
		}
	}
}