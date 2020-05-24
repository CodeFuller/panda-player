using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	public class RateSongsViewModel : ViewModelBase, IRateSongsViewModel
	{
		private readonly ISongsService songsService;

		private IReadOnlyCollection<SongModel> Songs { get; set; }

		public static IEnumerable<RatingModel> AvailableRatings => RatingModel.All;

		private RatingModel selectedRating;

		public RatingModel SelectedRating
		{
			get => selectedRating;
			set => Set(ref selectedRating, value);
		}

		public RateSongsViewModel(ISongsService songsService)
		{
			this.songsService = songsService ?? throw new ArgumentNullException(nameof(songsService));
		}

		public void Load(IEnumerable<SongModel> songs)
		{
			Songs = songs.ToList();
			SelectedRating = RatingModel.DefaultValue;
		}

		public async Task Save(CancellationToken cancellationToken)
		{
			foreach (var song in Songs)
			{
				song.Rating = SelectedRating;
				await songsService.UpdateSong(song, cancellationToken);
			}
		}
	}
}
