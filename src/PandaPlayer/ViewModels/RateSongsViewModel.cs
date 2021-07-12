using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.Internal;

namespace PandaPlayer.ViewModels
{
	public class RateSongsViewModel : ViewModelBase, IRateSongsViewModel
	{
		private readonly ISongsService songsService;

		private IReadOnlyCollection<SongModel> Songs { get; set; }

		public static IEnumerable<RatingModel> AvailableRatings => RatingHelpers.AllRatingValues
			.OrderByDescending(r => r);

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
			SelectedRating = RatingHelpers.DefaultValueProposedForAssignment;
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
