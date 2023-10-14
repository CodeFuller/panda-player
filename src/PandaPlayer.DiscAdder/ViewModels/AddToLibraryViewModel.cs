using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PandaPlayer.Core.Models;
using PandaPlayer.DiscAdder.MusicStorage;
using PandaPlayer.DiscAdder.ViewModels.Interfaces;
using PandaPlayer.DiscAdder.ViewModels.ViewModelItems;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Shared.Extensions;

namespace PandaPlayer.DiscAdder.ViewModels
{
	internal class AddToLibraryViewModel : ObservableObject, IAddToLibraryViewModel
	{
		private const int AddSongProgressStep = 5;
		private const int AddDiscImageProgressStep = 1;

		private readonly IWorkshopMusicStorage workshopMusicStorage;

		private readonly IFoldersService foldersService;
		private readonly IDiscsService discService;
		private readonly ISongsService songService;
		private readonly IArtistsService artistService;

		private IReadOnlyCollection<SongViewItem> SongsForAdding { get; set; }

		private IReadOnlyCollection<DiscImageViewItem> DiscImagesForAdding { get; set; }

		public string Name => "Add To Library";

		private bool dataIsReady;

		public bool DataIsReady
		{
			get => dataIsReady;
			set => SetProperty(ref dataIsReady, value);
		}

		private bool canAddContent;

		public bool CanAddContent
		{
			get => canAddContent;
			private set => SetProperty(ref canAddContent, value);
		}

		private bool deleteSourceContent;

		public bool DeleteSourceContent
		{
			get => deleteSourceContent;
			set => SetProperty(ref deleteSourceContent, value);
		}

		private int currentProgress;

		public int CurrentProgress
		{
			get => currentProgress;
			set
			{
				SetProperty(ref currentProgress, value);
				OnPropertyChanged(nameof(CurrentProgressPercentage));
			}
		}

		private int progressMaximum;

		public int ProgressMaximum
		{
			get => progressMaximum;
			set
			{
				SetProperty(ref progressMaximum, value);
				OnPropertyChanged(nameof(CurrentProgressPercentage));
			}
		}

		public string CurrentProgressPercentage => $"{(ProgressMaximum > 0 ? CurrentProgress / (double)ProgressMaximum * 100 : 0):N1}%";

		private string progressMessages;

		public string ProgressMessages
		{
			get => progressMessages;
			set => SetProperty(ref progressMessages, value);
		}

		public ICommand AddToLibraryCommand { get; }

		public AddToLibraryViewModel(IWorkshopMusicStorage workshopMusicStorage, IFoldersService foldersService,
			IDiscsService discService, ISongsService songService, IArtistsService artistService)
		{
			this.workshopMusicStorage = workshopMusicStorage ?? throw new ArgumentNullException(nameof(workshopMusicStorage));
			this.foldersService = foldersService ?? throw new ArgumentNullException(nameof(foldersService));
			this.discService = discService ?? throw new ArgumentNullException(nameof(discService));
			this.songService = songService ?? throw new ArgumentNullException(nameof(songService));
			this.artistService = artistService ?? throw new ArgumentNullException(nameof(artistService));

			AddToLibraryCommand = new AsyncRelayCommand(() => AddContentToLibrary(CancellationToken.None));
		}

		public void Load(IEnumerable<SongViewItem> songs, IEnumerable<DiscImageViewItem> images)
		{
			SongsForAdding = songs.ToList();
			DiscImagesForAdding = images.ToList();

			DataIsReady = false;
			CanAddContent = true;
			DeleteSourceContent = true;
		}

		private async Task AddContentToLibrary(CancellationToken cancellationToken)
		{
			CanAddContent = false;

			// Wrapping call with Task.Run() so that progress and logging messages in UI are updated during adding.
			await Task.Run(() => AddContentToLibraryInternal(cancellationToken), cancellationToken);

			DataIsReady = true;
		}

		private async Task AddContentToLibraryInternal(CancellationToken cancellationToken)
		{
			CurrentProgress = 0;
			ProgressMaximum = CountProgressMaximum();

			var createdDiscs = new Dictionary<string, DiscModel>();
			await AddSongsToLibrary(createdDiscs, cancellationToken);
			await AddDiscCoverImages(createdDiscs, cancellationToken);

			if (DeleteSourceContent)
			{
				ProgressMessages += "Deleting source content...\n";
				workshopMusicStorage.DeleteSourceContent(SongsForAdding.Select(s => s.SourceFilePath).Concat(DiscImagesForAdding.Select(im => im.ImageInfo.FileName)));
				ProgressMessages += "Source content was deleted successfully\n";
			}
		}

		private int CountProgressMaximum()
		{
			return (SongsForAdding.Count * AddSongProgressStep) + (DiscImagesForAdding.Count * AddDiscImageProgressStep);
		}

		private async Task AddSongsToLibrary(IDictionary<string, DiscModel> createdDiscs, CancellationToken cancellationToken)
		{
			var allArtists = await artistService.GetAllArtists(cancellationToken);
			var existingArtists = allArtists.ToDictionary(x => x.Name, x => x);

			foreach (var addedSong in SongsForAdding)
			{
				var songDisc = await ProvideSongDisc(addedSong, createdDiscs, cancellationToken);
				var songArtist = await ProvideSongArtist(addedSong.ArtistName, existingArtists, cancellationToken);

				var newSong = new SongModel
				{
					Title = addedSong.Title,
					TreeTitle = addedSong.TreeTitle,
					TrackNumber = addedSong.Track,
					Artist = songArtist,
					Genre = addedSong.Genre,
					Rating = null,
				};

				songDisc.AddSong(newSong);

				ProgressMessages += $"Adding song '{addedSong.SourceFilePath}'...\n";
				await using var songContent = File.OpenRead(addedSong.SourceFilePath);
				await songService.CreateSong(newSong, songContent, cancellationToken);

				CurrentProgress += AddSongProgressStep;
			}
		}

		private async Task<DiscModel> ProvideSongDisc(SongViewItem songItem, IDictionary<string, DiscModel> createdDiscs, CancellationToken cancellationToken)
		{
			if (songItem.ExistingDisc != null)
			{
				return songItem.ExistingDisc;
			}

			var discSourcePath = songItem.DiscSourcePath;

			if (createdDiscs.TryGetValue(discSourcePath, out var createdDisc))
			{
				return createdDisc;
			}

			createdDisc = await CreateDisc(songItem.DiscItem, cancellationToken);
			createdDiscs.Add(discSourcePath, createdDisc);

			return createdDisc;
		}

		private async Task<DiscModel> CreateDisc(DiscViewItem discItem, CancellationToken cancellationToken)
		{
			var discFolder = await ProvideDiscFolder(discItem.DestinationFolderPath, cancellationToken);

			var newDisc = new DiscModel
			{
				Year = discItem.Year,
				Title = discItem.DiscTitle,
				TreeTitle = discItem.TreeTitle,
				AlbumTitle = discItem.AlbumTitle,
			};

			discFolder.AddDisc(newDisc);

			ProgressMessages += $"Creating disc '{newDisc.TreeTitle}' ...\n";
			await discService.CreateDisc(newDisc, cancellationToken);

			return newDisc;
		}

		private async Task<ArtistModel> ProvideSongArtist(string artistName, IDictionary<string, ArtistModel> existingArtists, CancellationToken cancellationToken)
		{
			if (artistName == null)
			{
				return null;
			}

			if (existingArtists.TryGetValue(artistName, out var existingArtist))
			{
				return existingArtist;
			}

			var newArtist = new ArtistModel
			{
				Name = artistName,
			};

			ProgressMessages += $"Creating artist '{artistName}' ...\n";
			await artistService.CreateArtist(newArtist, cancellationToken);

			existingArtists.Add(artistName, newArtist);

			return newArtist;
		}

		private async Task AddDiscCoverImages(IReadOnlyDictionary<string, DiscModel> discs, CancellationToken cancellationToken)
		{
			foreach (var addedDiscImage in DiscImagesForAdding)
			{
				ProgressMessages += $"Adding disc image '{addedDiscImage.ImageInfo.FileName}'...\n";

				if (!discs.TryGetValue(addedDiscImage.DiscSourcePath, out var disc))
				{
					throw new InvalidOperationException($"Can not find disc with source path '{addedDiscImage.DiscSourcePath}'");
				}

				var newDiscImage = new DiscImageModel
				{
					TreeTitle = addedDiscImage.ImageInfo.GetDiscCoverImageTreeTitle(),
					ImageType = DiscImageType.Cover,
				};

				await using var imageContent = File.OpenRead(addedDiscImage.ImageInfo.FileName);
				await discService.SetDiscCoverImage(disc, newDiscImage, imageContent, cancellationToken);

				CurrentProgress += AddDiscImageProgressStep;
			}
		}

		private async Task<FolderModel> ProvideDiscFolder(IEnumerable<string> discFolderPath, CancellationToken cancellationToken)
		{
			var currentFolder = await foldersService.GetRootFolder(cancellationToken);

			var currentFolderDisplayFullPath = String.Empty;

			foreach (var currentSubfolderName in discFolderPath)
			{
				currentFolderDisplayFullPath += $"/{currentSubfolderName}";

				var currentSubfolder = currentFolder.Subfolders.SingleOrDefault(sf => String.Equals(sf.Name, currentSubfolderName, StringComparison.Ordinal));
				if (currentSubfolder == null)
				{
					ProgressMessages += $"Creating folder '{currentFolderDisplayFullPath}' ...\n";

					currentSubfolder = new FolderModel
					{
						Name = currentSubfolderName,
					};

					currentFolder.AddSubfolder(currentSubfolder);

					await foldersService.CreateEmptyFolder(currentSubfolder, cancellationToken);
				}

				currentFolder = currentSubfolder;
			}

			return currentFolder;
		}
	}
}
