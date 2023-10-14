using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.Internal;

namespace PandaPlayer.ViewModels
{
	public class RateSongsViewModel : ObservableObject, IRateSongsViewModel
	{
		private readonly ISongsService songsService;

		private IReadOnlyCollection<SongModel> Songs { get; set; }

		public IEnumerable<RatingModel> AvailableRatings => RatingHelpers.AllRatingValues.OrderByDescending(r => r);

		private RatingModel selectedRating;

		public RatingModel SelectedRating
		{
			get => selectedRating;
			set => SetProperty(ref selectedRating, value);
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
			void UpdateSong(SongModel song)
			{
				song.Rating = SelectedRating;
			}

			foreach (var song in Songs)
			{
				await songsService.UpdateSong(song, UpdateSong, cancellationToken);
			}
		}
	}
}
