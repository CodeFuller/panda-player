using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeFuller.Library.Wpf;
using GalaSoft.MvvmLight;
using PandaPlayer.Core.Models;
using PandaPlayer.DiscAdder.AddedContent;
using PandaPlayer.DiscAdder.MusicStorage;
using PandaPlayer.DiscAdder.ViewModels.Interfaces;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Media;
using PandaPlayer.Shared.Extensions;

namespace PandaPlayer.DiscAdder.ViewModels
{
	internal class AddToLibraryViewModel : ViewModelBase, IAddToLibraryViewModel
	{
		private readonly ISongMediaInfoProvider songMediaInfoProvider;
		private readonly IWorkshopMusicStorage workshopMusicStorage;

		private readonly IFoldersService foldersService;
		private readonly IDiscsService discService;
		private readonly ISongsService songService;
		private readonly IArtistsService artistService;

		private List<AddedSong> addedSongs;
		private List<AddedDiscImage> addedDiscImages;

		public string Name => "Add To Library";

		private bool dataIsReady;

		public bool DataIsReady
		{
			get => dataIsReady;
			set => Set(ref dataIsReady, value);
		}

		private bool deleteSourceContent;

		public bool DeleteSourceContent
		{
			get => deleteSourceContent;
			set => Set(ref deleteSourceContent, value);
		}

		private int currentProgress;

		public int CurrentProgress
		{
			get => currentProgress;
			set
			{
				Set(ref currentProgress, value);
				RaisePropertyChanged(nameof(CurrentProgressPercentage));
			}
		}

		private int progressMaximum;

		public int ProgressMaximum
		{
			get => progressMaximum;
			set
			{
				Set(ref progressMaximum, value);
				RaisePropertyChanged(nameof(CurrentProgressPercentage));
			}
		}

		public string CurrentProgressPercentage => $"{(ProgressMaximum > 0 ? CurrentProgress / (double)ProgressMaximum * 100 : 0):N1}%";

		private string progressMessages;

		public string ProgressMessages
		{
			get => progressMessages;
			set => Set(ref progressMessages, value);
		}

		public ICommand AddToLibraryCommand { get; }

		public AddToLibraryViewModel(ISongMediaInfoProvider songMediaInfoProvider, IWorkshopMusicStorage workshopMusicStorage,
			IFoldersService foldersService, IDiscsService discService, ISongsService songService, IArtistsService artistService)
		{
			this.songMediaInfoProvider = songMediaInfoProvider ?? throw new ArgumentNullException(nameof(songMediaInfoProvider));
			this.workshopMusicStorage = workshopMusicStorage ?? throw new ArgumentNullException(nameof(workshopMusicStorage));
			this.foldersService = foldersService ?? throw new ArgumentNullException(nameof(foldersService));
			this.discService = discService ?? throw new ArgumentNullException(nameof(discService));
			this.songService = songService ?? throw new ArgumentNullException(nameof(songService));
			this.artistService = artistService ?? throw new ArgumentNullException(nameof(artistService));

			AddToLibraryCommand = new AsyncRelayCommand(() => AddContentToLibrary(CancellationToken.None));
		}

		public void SetSongs(IEnumerable<AddedSong> songs)
		{
			addedSongs = songs.ToList();

			deleteSourceContent = false;
		}

		public void SetDiscsImages(IEnumerable<AddedDiscImage> images)
		{
			addedDiscImages = images.ToList();
		}

		private async Task AddContentToLibrary(CancellationToken cancellationToken)
		{
			if (addedSongs == null)
			{
				throw new InvalidOperationException("No songs for adding to the library");
			}

			DataIsReady = false;

			await Task.Run(() => AddContentToLibraryInternal(cancellationToken), cancellationToken);

			DataIsReady = true;
		}

		private async Task AddContentToLibraryInternal(CancellationToken cancellationToken)
		{
			CurrentProgress = 0;
			ProgressMaximum = 0;
			ProgressMaximum += await FillSongsMediaData(true);
			ProgressMaximum += await AddSongsToLibrary(true, cancellationToken);
			ProgressMaximum += await AddDiscCoverImages(true, cancellationToken);

			await FillSongsMediaData(false);
			await AddSongsToLibrary(false, cancellationToken);
			await AddDiscCoverImages(false, cancellationToken);

			if (DeleteSourceContent)
			{
				ProgressMessages += "Deleting source content...\n";
				workshopMusicStorage.DeleteSourceContent(addedSongs.Select(s => s.SourceFileName).Concat(addedDiscImages.Select(im => im.ImageInfo.FileName)));
				ProgressMessages += "Source content was deleted successfully\n";
			}
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
				ProgressMessages += $"Getting media info for '{addedSong.SourceFileName}'...\n";
				var mediaInfo = await songMediaInfoProvider.GetSongMediaInfo(addedSong.SourceFileName);

				addedSong.Song.BitRate = mediaInfo.Bitrate;
				addedSong.Song.Duration = mediaInfo.Duration;

				CurrentProgress += progressIncrement;
			}

			return taskProgressSize;
		}

		private async Task<int> AddSongsToLibrary(bool onlyCountProgressSize, CancellationToken cancellationToken)
		{
			const int progressIncrement = 5;
			var taskProgressSize = 0;

			// If new folder or disc is added, then same instance will be shared across all songs.
			// That is why we need no additional checks for created objects.
			// However, for artists there could be different instances of ArtistModel for the same new artist.
			// That is why we remember artists, which were created.
			var createdArtists = new Dictionary<string, ItemId>();

			foreach (var addedSong in addedSongs)
			{
				if (!onlyCountProgressSize)
				{
					var song = addedSong.Song;

					await ProvideSongArtist(song, createdArtists, cancellationToken);

					var songDisc = song.Disc;
					if (songDisc.Id == null)
					{
						await CreateDisc(songDisc, addedSong.Disc.FolderPath, cancellationToken);
					}

					ProgressMessages += $"Adding song '{addedSong.SourceFileName}'...\n";

					await using var songContent = File.OpenRead(addedSong.SourceFileName);
					await songService.CreateSong(song, songContent, cancellationToken);

					CurrentProgress += progressIncrement;
				}

				taskProgressSize += progressIncrement;
			}

			return taskProgressSize;
		}

		private async Task<int> AddDiscCoverImages(bool onlyCountProgressSize, CancellationToken cancellationToken)
		{
			const int progressIncrement = 1;
			var taskProgressSize = 0;

			if (addedDiscImages == null)
			{
				return taskProgressSize;
			}

			foreach (var image in addedDiscImages)
			{
				if (!onlyCountProgressSize)
				{
					ProgressMessages += $"Adding disc image '{image.ImageInfo.FileName}'...\n";

					var discImage = new DiscImageModel
					{
						Disc = image.Disc,
						TreeTitle = image.ImageInfo.GetDiscCoverImageTreeTitle(),
						ImageType = DiscImageType.Cover,
					};

					using var imageContent = File.OpenRead(image.ImageInfo.FileName);
					await discService.SetDiscCoverImage(discImage, imageContent, cancellationToken);

					CurrentProgress += progressIncrement;
				}

				taskProgressSize += progressIncrement;
			}

			return taskProgressSize;
		}

		private async Task ProvideSongArtist(SongModel song, IDictionary<string, ItemId> createdArtists, CancellationToken cancellationToken)
		{
			var artist = song.Artist;

			if (artist == null || artist.Id != null)
			{
				return;
			}

			if (createdArtists.TryGetValue(artist.Name, out var artistId))
			{
				artist.Id = artistId;
				return;
			}

			ProgressMessages += $"Creating artist '{artist.Name}' ...\n";
			await artistService.CreateArtist(artist, cancellationToken);

			createdArtists.Add(artist.Name, artist.Id);
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
					ProgressMessages += $"Creating folder '{currentFolderFullPath}' ...\n";

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
			disc.Folder ??= await CreateFolder(discFolderPath, cancellationToken);

			ProgressMessages += $"Creating disc '{disc.TreeTitle}' ...\n";

			await discService.CreateDisc(disc, cancellationToken);
		}
	}
}
