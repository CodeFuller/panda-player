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

namespace PandaPlayer.DiscAdder.ViewModels
{
	internal class EditSourceContentViewModel : ViewModelBase, IEditSourceContentViewModel
	{
		public string Name => "Edit Source Content";

		private readonly IContentCrawler contentCrawler;
		private readonly IReferenceContentParser referenceContentParser;
		private readonly ISourceContentChecker sourceContentChecker;
		private readonly IWorkshopMusicStorage workshopMusicStorage;

		private readonly DiscAdderSettings settings;

		public IRawReferenceContentViewModel RawReferenceContent { get; }

		public IReferenceContentViewModel ReferenceContent { get; }

		public IActualContentViewModel ActualContent { get; }

		public IEnumerable<AddedDiscInfo> AddedDiscs => ActualContent.Discs.Select(d => workshopMusicStorage.GetAddedDiscInfo(d.DiscDirectory, d.SongFileNames));

		public ICommand ReloadReferenceContentCommand { get; }

		public ICommand ReloadActualContentCommand { get; }

		public ICommand ReloadAllContentCommand { get; }

		private bool dataIsReady;

		public bool DataIsReady
		{
			get => dataIsReady;
			set => Set(ref dataIsReady, value);
		}

		public EditSourceContentViewModel(IContentCrawler contentCrawler, IReferenceContentParser referenceContentParser,
			ISourceContentChecker sourceContentChecker, IWorkshopMusicStorage workshopMusicStorage, IRawReferenceContentViewModel rawReferenceContent,
			IReferenceContentViewModel referenceContentViewModel, IActualContentViewModel actualContentViewModel, IOptions<DiscAdderSettings> options)
		{
			this.contentCrawler = contentCrawler ?? throw new ArgumentNullException(nameof(contentCrawler));
			this.referenceContentParser = referenceContentParser ?? throw new ArgumentNullException(nameof(referenceContentParser));
			this.sourceContentChecker = sourceContentChecker ?? throw new ArgumentNullException(nameof(sourceContentChecker));
			this.workshopMusicStorage = workshopMusicStorage ?? throw new ArgumentNullException(nameof(workshopMusicStorage));
			RawReferenceContent = rawReferenceContent ?? throw new ArgumentNullException(nameof(rawReferenceContent));
			ReferenceContent = referenceContentViewModel ?? throw new ArgumentNullException(nameof(referenceContentViewModel));
			ActualContent = actualContentViewModel ?? throw new ArgumentNullException(nameof(actualContentViewModel));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

			RawReferenceContent.PropertyChanged += OnRawReferenceContentChanged;

			ReloadReferenceContentCommand = new RelayCommand(ReloadReferenceContent);
			ReloadActualContentCommand = new RelayCommand(ReloadActualContent);
			ReloadAllContentCommand = new RelayCommand(ReloadAllContent);

			Messenger.Default.Register<ActualContentChangedEventArgs>(this, OnActualContentChanged);
		}

		public async Task Load(CancellationToken cancellationToken)
		{
			await RawReferenceContent.LoadContent(cancellationToken);

			ReloadActualContent();
		}

		public Task ResetContent(CancellationToken cancellationToken)
		{
			return RawReferenceContent.ClearContent(cancellationToken);
		}

		private void OnActualContentChanged(ActualContentChangedEventArgs message)
		{
			UpdateContentCorrectness();
		}

		private void ReloadReferenceContent()
		{
			var contentBuilder = new StringBuilder();
			foreach (var disc in ActualContent.Discs)
			{
				contentBuilder.AppendLine(CultureInfo.InvariantCulture, $"# {disc.DiscDirectory}");
				contentBuilder.AppendLine();
			}

			RawReferenceContent.Content = contentBuilder.ToString();
		}

		private void ReloadActualContent()
		{
			var discs = contentCrawler.LoadDiscs(settings.WorkshopStoragePath);

			ActualContent.SetContent(discs);
			UpdateContentCorrectness();
		}

		private void ReloadAllContent()
		{
			// Actual content should be reloaded first, because reference content is initialized based on actual content.
			ReloadActualContent();

			ReloadReferenceContent();
		}

		private void OnRawReferenceContentChanged(object sender, PropertyChangedEventArgs e)
		{
			var discs = referenceContentParser.Parse(RawReferenceContent.Content);

			ReferenceContent.SetExpectedDiscs(discs);
			UpdateContentCorrectness();
		}

		private void SetContentCorrectness()
		{
			sourceContentChecker.SetContentCorrectness(ReferenceContent, ActualContent);
		}

		private void UpdateContentCorrectness()
		{
			SetContentCorrectness();
			DataIsReady = !ReferenceContent.ContentIsIncorrect && !ActualContent.ContentIsIncorrect;
		}
	}
}
