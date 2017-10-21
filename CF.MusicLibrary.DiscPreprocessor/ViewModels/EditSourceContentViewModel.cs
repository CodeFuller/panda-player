using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.MusicLibrary.DiscPreprocessor.Events;
using CF.MusicLibrary.DiscPreprocessor.Interfaces;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using CF.MusicLibrary.DiscPreprocessor.ParsingContent;
using CF.MusicLibrary.DiscPreprocessor.ViewModels.Interfaces;
using CF.MusicLibrary.DiscPreprocessor.ViewModels.SourceContent;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using static System.FormattableString;

namespace CF.MusicLibrary.DiscPreprocessor.ViewModels
{
	public class EditSourceContentViewModel : ViewModelBase, IEditSourceContentViewModel
	{
		public string Name => "Edit Source Content";

		private readonly IContentCrawler contentCrawler;
		private readonly IDiscContentParser discContentParser;
		private readonly IDiscContentComparer discContentComparer;
		private readonly IWorkshopMusicStorage workshopMusicStorage;

		public EthalonContentViewModel RawEthalonDiscs { get; }

		public DiscTreeViewModel EthalonDiscs { get; }

		public DiscTreeViewModel CurrentDiscs { get; }

		public IEnumerable<AddedDiscInfo> AddedDiscs => CurrentDiscs.Select(d => workshopMusicStorage.GetAddedDiscInfo(d.DiscDirectory, d.SongFileNames));

		public ICommand ReloadRawContentCommand { get; }

		private bool dataIsReady;
		public bool DataIsReady
		{
			get { return dataIsReady; }
			set { Set(ref dataIsReady, value); }
		}

		public EditSourceContentViewModel(IContentCrawler contentCrawler, IDiscContentParser discContentParser, IDiscContentComparer discContentComparer,
			IWorkshopMusicStorage workshopMusicStorage, IFileSystemFacade fileSystemFacade)
		{
			if (contentCrawler == null)
			{
				throw new ArgumentNullException(nameof(contentCrawler));
			}
			if (discContentParser == null)
			{
				throw new ArgumentNullException(nameof(discContentParser));
			}
			if (discContentComparer == null)
			{
				throw new ArgumentNullException(nameof(discContentComparer));
			}
			if (workshopMusicStorage == null)
			{
				throw new ArgumentNullException(nameof(workshopMusicStorage));
			}
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}

			this.contentCrawler = contentCrawler;
			this.discContentParser = discContentParser;
			this.discContentComparer = discContentComparer;
			this.workshopMusicStorage = workshopMusicStorage;

			EthalonDiscs = new DiscTreeViewModel();
			CurrentDiscs = new DiscTreeViewModel();

			string appDataPath = AppSettings.GetRequiredValue<string>("AppDataPath");
			RawEthalonDiscs = new EthalonContentViewModel(fileSystemFacade, appDataPath);
			RawEthalonDiscs.PropertyChanged += OnRawEthalonDiscsPropertyChanged;

			ReloadRawContentCommand = new RelayCommand(ReloadRawContent);

			Messenger.Default.Register<DiscContentChangedEventArgs>(this, OnDiscContentChanged);
		}

		public void LoadDefaultContent()
		{
			RawEthalonDiscs.LoadRawEthalonDiscsContent();

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

			RawEthalonDiscs.Content = contentBuilder.ToString();
		}

		private void OnRawEthalonDiscsPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateDiscs(EthalonDiscs, discContentParser.Parse(RawEthalonDiscs.Content));
		}

		public void LoadCurrentDiscs()
		{
			var discs = contentCrawler.LoadDiscs(AppSettings.GetRequiredValue<string>("WorkshopDirectory")).ToList();

			UpdateDiscs(CurrentDiscs, discs);
		}

		private void UpdateDiscs(DiscTreeViewModel discs, IEnumerable<DiscContent> newDiscs)
		{
			discs.SetDiscs(newDiscs);
			UpdateContentCorrectness();
		}

		private void SetContentCorrectness()
		{
			discContentComparer.SetDiscsCorrectness(EthalonDiscs, CurrentDiscs);
		}

		private void UpdateContentCorrectness()
		{
			SetContentCorrectness();
			DataIsReady = !EthalonDiscs.ContentIsIncorrect && !CurrentDiscs.ContentIsIncorrect;
		}
	}
}
