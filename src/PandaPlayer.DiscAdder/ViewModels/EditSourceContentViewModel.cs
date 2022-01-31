﻿using System;
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
			IWorkshopMusicStorage workshopMusicStorage, IReferenceContentViewModel rawReferenceDiscs, IOptions<DiscAdderSettings> options)
		{
			this.contentCrawler = contentCrawler ?? throw new ArgumentNullException(nameof(contentCrawler));
			this.discContentParser = discContentParser ?? throw new ArgumentNullException(nameof(discContentParser));
			this.discContentComparer = discContentComparer ?? throw new ArgumentNullException(nameof(discContentComparer));
			this.workshopMusicStorage = workshopMusicStorage ?? throw new ArgumentNullException(nameof(workshopMusicStorage));
			RawReferenceDiscs = rawReferenceDiscs ?? throw new ArgumentNullException(nameof(rawReferenceDiscs));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

			ReferenceDiscs = new DiscTreeViewModel();
			CurrentDiscs = new DiscTreeViewModel();

			RawReferenceDiscs.PropertyChanged += OnRawReferenceDiscsPropertyChanged;

			ReloadRawContentCommand = new RelayCommand(ReloadRawContent);

			Messenger.Default.Register<DiscContentChangedEventArgs>(this, OnDiscContentChanged);
		}

		public async Task LoadDefaultContent(CancellationToken cancellationToken)
		{
			await RawReferenceDiscs.LoadRawReferenceDiscsContent(cancellationToken);

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
				contentBuilder.Append(CultureInfo.InvariantCulture, $"# {disc.DiscDirectory}");
				contentBuilder.AppendLine();
				contentBuilder.AppendLine();
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
