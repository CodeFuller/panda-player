using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;
using PandaPlayer.ViewModels;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.Views.DesignInstances
{
	internal class EditSongPropertiesDesignData : IEditSongPropertiesViewModel
	{
		public bool SingleSongMode => true;

		public string TreeTitle { get; set; } = "Some Tree Title";

		public string Title { get; set; } = "Some Title";

		public EditedSongProperty<ArtistModel> Artist { get; set; }

		public string NewArtistName { get; set; }

		public EditedSongProperty<GenreModel> Genre { get; set; }

		public short? TrackNumber { get; set; } = 7;

		public IReadOnlyCollection<EditedSongProperty<ArtistModel>> AvailableArtists { get; }

		public IReadOnlyCollection<EditedSongProperty<GenreModel>> AvailableGenres { get; }

		public EditSongPropertiesDesignData()
		{
			AvailableArtists = new[]
			{
				new EditedSongProperty<ArtistModel>(new ArtistModel
				{
					Id = new ItemId("1"),
					Name = "Neuro Dubel",
				}),

				new EditedSongProperty<ArtistModel>(new ArtistModel
				{
					Id = new ItemId("2"),
					Name = "Nautilus Pompilius",
				}),
			};

			AvailableGenres = new[]
			{
				new EditedSongProperty<GenreModel>(new GenreModel
				{
					Id = new ItemId("1"),
					Name = "Rock",
				}),

				new EditedSongProperty<GenreModel>(new GenreModel
				{
					Id = new ItemId("2"),
					Name = "Symphonic Metal",
				}),
			};

			Artist = AvailableArtists.First();
			Genre = AvailableGenres.First();
		}

		public Task Load(IEnumerable<SongModel> songs, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task Save(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
