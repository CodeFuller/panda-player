using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CF.Library.Core.Facades;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Options;
using MusicLibrary.DiscAdder.Events;
using MusicLibrary.DiscAdder.Interfaces;
using MusicLibrary.DiscAdder.MusicStorage;
using MusicLibrary.DiscAdder.ParsingContent;
using MusicLibrary.DiscAdder.ViewModels.Interfaces;
using MusicLibrary.DiscAdder.ViewModels.SourceContent;
using static System.FormattableString;

namespace MusicLibrary.DiscAdder.ViewModels
{
	internal class EditSourceContentViewModel : ViewModelBase, IEditSourceContentViewModel
	{
		public string Name => "Edit Source Content";

		private readonly IContentCrawler contentCrawler;
		private readonly IDiscContentParser discContentParser;
		private readonly IDiscContentComparer discContentComparer;
		private readonly IWorkshopMusicStorage workshopMusicStorage;

		private readonly DiscAdderSettings settings;

		public ReferenceContentViewModel RawReferenceDiscs { get; }

		public DiscTreeViewModel ReferenceDiscs { get; }

		public DiscTreeViewModel CurrentDiscs { get; }

		public IEnumerable<AddedDiscInfo> AddedDiscs => CurrentDiscs.Select(d => workshopMusicStorage.GetAddedDiscInfo(d.DiscDirectory, d.SongFileNames));

		public ICommand ReloadRawContentCommand { get; }

		private bool dataIsReady;

		public bool DataIsReady
		{
			get => dataIsReady;
			set => Set(ref dataIsReady, value);
		}

		public EditSourceContentViewModel(IContentCrawler contentCrawler, IDiscContentParser discContentParser, IDiscContentComparer discContentComparer,
			IWorkshopMusicStorage workshopMusicStorage, IFileSystemFacade fileSystemFacade, IOptions<DiscAdderSettings> options)
		{
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}

			this.contentCrawler = contentCrawler ?? throw new ArgumentNullException(nameof(contentCrawler));
			this.discContentParser = discContentParser ?? throw new ArgumentNullException(nameof(discContentParser));
			this.discContentComparer = discContentComparer ?? throw new ArgumentNullException(nameof(discContentComparer));
			this.workshopMusicStorage = workshopMusicStorage ?? throw new ArgumentNullException(nameof(workshopMusicStorage));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

			ReferenceDiscs = new DiscTreeViewModel();
			CurrentDiscs = new DiscTreeViewModel();

			RawReferenceDiscs = new ReferenceContentViewModel(fileSystemFacade, settings.DataStoragePath);
			RawReferenceDiscs.PropertyChanged += OnRawReferenceDiscsPropertyChanged;

			ReloadRawContentCommand = new RelayCommand(ReloadRawContent);

			Messenger.Default.Register<DiscContentChangedEventArgs>(this, OnDiscContentChanged);
		}

		public void LoadDefaultContent()
		{
			RawReferenceDiscs.LoadRawEthalonDiscsContent();

			LoadCurrentDiscs();
		}

		private void OnDiscContentChanged(DiscContentChangedEventArgs message)
		{
			UpdateContentCorrectness();
		}

		public void ReloadRawContent()
		{
			var contentBuilder = new StringBuilder();
			foreach (var disc in CurrentDiscs.Discs)
			{
				contentBuilder.Append(Invariant($"# {disc.DiscDirectory}\n\n"));
			}

			RawReferenceDiscs.Content = contentBuilder.ToString();
		}

		private void OnRawReferenceDiscsPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateDiscs(ReferenceDiscs, discContentParser.Parse(RawReferenceDiscs.Content));
		}

		public void LoadCurrentDiscs()
		{
			var discs = contentCrawler.LoadDiscs(settings.WorkshopStoragePath).ToList();

			UpdateDiscs(CurrentDiscs, discs);
		}

		private void UpdateDiscs(DiscTreeViewModel discs, IEnumerable<DiscContent> newDiscs)
		{
			discs.SetDiscs(newDiscs);
			UpdateContentCorrectness();
		}

		private void SetContentCorrectness()
		{
			discContentComparer.SetDiscsCorrectness(ReferenceDiscs, CurrentDiscs);
		}

		private void UpdateContentCorrectness()
		{
			SetContentCorrectness();
			DataIsReady = !ReferenceDiscs.ContentIsIncorrect && !CurrentDiscs.ContentIsIncorrect;
		}
	}
}
