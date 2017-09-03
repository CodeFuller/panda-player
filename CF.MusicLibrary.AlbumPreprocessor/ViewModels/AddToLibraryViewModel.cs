using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Wpf;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels.Interfaces;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.Tagger;
using GalaSoft.MvvmLight;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class AddToLibraryViewModel : ViewModelBase, IAddToLibraryViewModel
	{
		private readonly ISongTagger songTagger;
		private readonly IMusicLibrary musicLibrary;
		private readonly IFileSystemFacade fileSystemFacade;

		private List<TaggedSongData> songsTagData;
		private List<AddedAlbumCoverImage> albumCoverImages;

		public string Name => "Add To Library";

		private bool dataIsReady;
		public bool DataIsReady 
		{
			get { return dataIsReady; }
			set { Set(ref dataIsReady, value); }
		}

		public ICommand AddToLibraryCommand { get; }

		private int currProgress;
		public int CurrProgress
		{
			get { return currProgress; }
			set { Set(ref currProgress, value); }
		}

		private int progressSize;
		public int ProgressSize
		{
			get { return progressSize; }
			set { Set(ref progressSize, value); }
		}

		private string progressMessages;
		public string ProgressMessages
		{
			get { return progressMessages; }
			set { Set(ref progressMessages, value); }
		}

		public void SetSongsTagData(IEnumerable<TaggedSongData> tagData)
		{
			songsTagData = tagData.ToList();
		}

		public void SetAlbumCoverImages(IEnumerable<AddedAlbumCoverImage> coverImages)
		{
			albumCoverImages = coverImages.ToList();
		}

		public AddToLibraryViewModel(ISongTagger songTagger, IMusicLibrary musicLibrary, IFileSystemFacade fileSystemFacade)
		{
			if (songTagger == null)
			{
				throw new ArgumentNullException(nameof(songTagger));
			}
			if (musicLibrary == null)
			{
				throw new ArgumentNullException(nameof(musicLibrary));
			}
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}

			this.songTagger = songTagger;
			this.musicLibrary = musicLibrary;
			this.fileSystemFacade = fileSystemFacade;

			AddToLibraryCommand = new AsyncRelayCommand(AddContentToLibrary);
		}

		public async Task AddContentToLibrary()
		{
			DataIsReady = false;

			CurrProgress = 0;
			ProgressSize = await StoreContentInLibrary(true);
			await StoreContentInLibrary(false);

			if (AppSettings.GetRequiredValue<bool>("DeleteSourceContentAfterAdding"))
			{
				DeleteSourceDirTree();
			}

			DataIsReady = true;
		}

		private async Task<int> StoreContentInLibrary(bool onlyCountProgressSize)
		{
			int taskProgressSize = 0;
			taskProgressSize += SetTags(onlyCountProgressSize);
			taskProgressSize += await StoreAlbumsInLibrary(onlyCountProgressSize);
			return taskProgressSize;
		}

		private int SetTags(bool onlyCountProgressSize)
		{
			int taskProgressSize = 0;
			foreach (TaggedSongData song in songsTagData)
			{
				int currProgressInc = 1;
				if (!onlyCountProgressSize)
				{
					ProgressMessages += Current($"Updating tags for '{song.SourceFileName}'...\n");
					fileSystemFacade.ClearReadOnlyAttribute(song.SourceFileName);
					songTagger.SetTagData(song.SourceFileName, song);
					CurrProgress += currProgressInc;
				}

				taskProgressSize += currProgressInc;
			}

			return taskProgressSize;
		}

		private async Task<int> StoreAlbumsInLibrary(bool onlyCountProgressSize)
		{
			if (songsTagData == null)
			{
				throw new InvalidOperationException("No songs for adding to the library");
			}

			int taskProgressSize = 0;

			foreach (TaggedSongData song in songsTagData)
			{
				int currProgressInc = 1;
				if (!onlyCountProgressSize)
				{
					ProgressMessages += Current($"Adding to library '{song.SourceFileName}'...\n");

					//	Currently we don't add song to IMusicCatalog, only to IMusicStorage.
					//	If this is changed we should fill all other Song fields like Artist, Title, FileSize, Bitrate, ...
					Song addedSong = new Song
					{
						Uri = song.StorageUri
					};
					await musicLibrary.AddSong(addedSong, song.SourceFileName);
					CurrProgress += currProgressInc;
				}

				taskProgressSize += currProgressInc;
			}

			if (albumCoverImages != null)
			{
				foreach (AddedAlbumCoverImage coverImage in albumCoverImages)
				{
					int currProgressInc = 1;
					if (!onlyCountProgressSize)
					{
						ProgressMessages += Current($"Adding album cover image '{coverImage.CoverImageFileName}'...\n");
						await musicLibrary.SetAlbumCoverImage(coverImage.AlbumStorageUri, coverImage.CoverImageFileName);
						CurrProgress += currProgressInc;
					}

					taskProgressSize += currProgressInc;
				}
			}

			return taskProgressSize;
		}

		private void DeleteSourceDirTree()
		{
			foreach (var subDirectory in fileSystemFacade.EnumerateDirectories(AppSettings.GetRequiredValue<string>("WorkshopDirectory")))
			{
				List<string> files = new List<string>();
				FindDirectoryFiles(subDirectory, files);

				if (files.Any())
				{
					return;
				}

				ProgressMessages += Current($"Deleting source directory '{subDirectory}'...\n");
				fileSystemFacade.DeleteDirectory(subDirectory, true);
			}
		}

		private void FindDirectoryFiles(string directoryPath, List<string> files)
		{
			foreach (string subDirectory in fileSystemFacade.EnumerateDirectories(directoryPath))
			{
				FindDirectoryFiles(subDirectory, files);
			}

			foreach (string file in fileSystemFacade.EnumerateFiles(directoryPath))
			{
				files.Add(file);
			}
		}
	}
}
