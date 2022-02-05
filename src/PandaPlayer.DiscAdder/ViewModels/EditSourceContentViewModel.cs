using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Options;
using PandaPlayer.DiscAdder.Events;
using PandaPlayer.DiscAdder.Interfaces;
using PandaPlayer.DiscAdder.MusicStorage;
using PandaPlayer.DiscAdder.ParsingContent;
using PandaPlayer.DiscAdder.ViewModels.Interfaces;
using PandaPlayer.DiscAdder.ViewModels.SourceContent;

namespace PandaPlayer.DiscAdder.ViewModels
{
	internal class EditSourceContentViewModel : ViewModelBase, IEditSourceContentViewModel
	{
		public string Name => "Edit Source Content";

		private readonly IContentCrawler contentCrawler;
		private readonly IDiscContentParser discContentParser;
		private readonly IDiscContentComparer discContentComparer;
		private readonly IWorkshopMusicStorage workshopMusicStorage;

		private readonly DiscAdderSettings settings;

		public IReferenceContentViewModel RawReferenceDiscs { get; }

		public DiscTreeViewModel ReferenceContent { get; }

		public DiscTreeViewModel DiskContent { get; }

		public IEnumerable<AddedDiscInfo> AddedDiscs => DiskContent.Select(d => workshopMusicStorage.GetAddedDiscInfo(d.DiscDirectory, d.SongFileNames));

		public ICommand ReloadReferenceContentCommand { get; }

		public ICommand ReloadDiskContentCommand { get; }

		public ICommand ReloadAllContentCommand { get; }

		private bool dataIsReady;

		public bool DataIsReady
		{
			get => dataIsReady;
			set => Set(ref dataIsReady, value);
		}

		public EditSourceContentViewModel(IContentCrawler contentCrawler, IDiscContentParser discContentParser, IDiscContentComparer discContentComparer,
			IWorkshopMusicStorage workshopMusicStorage, IReferenceContentViewModel rawReferenceDiscs, IOptions<DiscAdderSettings> options)
		{
			this.contentCrawler = contentCrawler ?? throw new ArgumentNullException(nameof(contentCrawler));
			this.discContentParser = discContentParser ?? throw new ArgumentNullException(nameof(discContentParser));
			this.discContentComparer = discContentComparer ?? throw new ArgumentNullException(nameof(discContentComparer));
			this.workshopMusicStorage = workshopMusicStorage ?? throw new ArgumentNullException(nameof(workshopMusicStorage));
			RawReferenceDiscs = rawReferenceDiscs ?? throw new ArgumentNullException(nameof(rawReferenceDiscs));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

			ReferenceContent = new DiscTreeViewModel();
			DiskContent = new DiscTreeViewModel();

			RawReferenceDiscs.PropertyChanged += OnRawReferenceDiscsPropertyChanged;

			ReloadReferenceContentCommand = new RelayCommand(ReloadReferenceContent);
			ReloadDiskContentCommand = new RelayCommand(ReloadDiskContent);
			ReloadAllContentCommand = new RelayCommand(ReloadAllContent);

			Messenger.Default.Register<DiskContentChangedEventArgs>(this, OnDiskContentChanged);
		}

		public async Task LoadDefaultContent(CancellationToken cancellationToken)
		{
			await RawReferenceDiscs.LoadRawReferenceDiscsContent(cancellationToken);

			ReloadDiskContent();
		}

		private void OnDiskContentChanged(DiskContentChangedEventArgs message)
		{
			UpdateContentCorrectness();
		}

		private void ReloadReferenceContent()
		{
			var contentBuilder = new StringBuilder();
			foreach (var disc in DiskContent.Discs)
			{
				contentBuilder.AppendLine(CultureInfo.InvariantCulture, $"# {disc.DiscDirectory}");
				contentBuilder.AppendLine();
			}

			RawReferenceDiscs.Content = contentBuilder.ToString();
		}

		private void ReloadDiskContent()
		{
			var discs = contentCrawler.LoadDiscs(settings.WorkshopStoragePath);

			UpdateDiscTree(DiskContent, discs);
		}

		private void ReloadAllContent()
		{
			// Disk content should be reloaded first, because reference content is initialized based on disk content.
			ReloadDiskContent();

			ReloadReferenceContent();
		}

		private void OnRawReferenceDiscsPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateDiscTree(ReferenceContent, discContentParser.Parse(RawReferenceDiscs.Content));
		}

		private void UpdateDiscTree(DiscTreeViewModel discs, IEnumerable<DiscContent> newDiscs)
		{
			discs.SetDiscs(newDiscs);
			UpdateContentCorrectness();
		}

		private void SetContentCorrectness()
		{
			discContentComparer.SetDiscsCorrectness(ReferenceContent, DiskContent);
		}

		private void UpdateContentCorrectness()
		{
			SetContentCorrectness();
			DataIsReady = !ReferenceContent.ContentIsIncorrect && !DiskContent.ContentIsIncorrect;
		}
	}
}
