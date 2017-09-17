using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.DiscAdviser;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class DiscAdviserViewModel : ViewModelBase, IDiscAdviserViewModel
	{
		private const int AdvisedDiscsNumber = 30;

		private readonly DiscLibrary discLibrary;
		private readonly IDiscAdviser discAdviser;

		private readonly List<Disc> currAdvisedDsics = new List<Disc>();

		private int currAdvisedDiscIndex;
		private int CurrAdvisedDiscIndex
		{
			get { return currAdvisedDiscIndex; }
			set
			{
				currAdvisedDiscIndex = value;
				RaisePropertyChanged(nameof(CurrentDisc));
				RaisePropertyChanged(nameof(CurrentDiscAnnouncement));
			}
		}

		public Disc CurrentDisc => CurrAdvisedDiscIndex < currAdvisedDsics.Count ? currAdvisedDsics[CurrAdvisedDiscIndex] : null;

		public string CurrentDiscAnnouncement
		{
			get
			{
				var currDisc = CurrentDisc;
				if (currDisc == null)
				{
					return "N/A";
				}

				return currDisc.Artist == null ? currDisc.Title : Current($"{currDisc.Artist.Name} - {currDisc.Title}");
			}
		}

		public ICommand PlayCurrentDiscCommand { get; }

		public ICommand SwitchToNextDiscCommand { get; }

		public DiscAdviserViewModel(DiscLibrary discLibrary, IDiscAdviser discAdviser)
		{
			if (discLibrary == null)
			{
				throw new ArgumentNullException(nameof(discLibrary));
			}
			if (discAdviser == null)
			{
				throw new ArgumentNullException(nameof(discAdviser));
			}

			this.discLibrary = discLibrary;
			this.discAdviser = discAdviser;

			PlayCurrentDiscCommand = new RelayCommand(PlayCurrentDisc);
			SwitchToNextDiscCommand = new RelayCommand(SwitchToNextDisc);

			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, e => OnPlaylistFinished(e.Playlist));
		}

		public void Load()
		{
			RebuildAdvisedDiscs();
		}

		private void PlayCurrentDisc()
		{
			var disc = CurrentDisc;
			if (disc != null)
			{
				Messenger.Default.Send(new PlayDiscEventArgs(disc));
			}
		}

		private void SwitchToNextDisc()
		{
			if (++CurrAdvisedDiscIndex >= currAdvisedDsics.Count)
			{
				RebuildAdvisedDiscs();
			}
		}

		private void RebuildAdvisedDiscs()
		{
			currAdvisedDsics.Clear();
			currAdvisedDsics.AddRange(discAdviser.AdviseDiscs(discLibrary).Take(AdvisedDiscsNumber));
			CurrAdvisedDiscIndex = 0;
		}

		private void OnPlaylistFinished(ISongPlaylistViewModel playlist)
		{
			var playedDisc = playlist.PlayedDisc;
			if (playedDisc != null && playedDisc == CurrentDisc)
			{
				SwitchToNextDisc();
			}
		}
	}
}
