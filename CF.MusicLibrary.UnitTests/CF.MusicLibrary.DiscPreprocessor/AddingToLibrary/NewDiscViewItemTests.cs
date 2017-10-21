using System;
using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Attributes;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	[TestFixture]
	public class NewDiscViewItemTests
	{
		[ExcludeFromTestCoverage("Empty stub of base abstract class")]
		private class ConcreteDiscViewItem : NewDiscViewItem
		{
			public ConcreteDiscViewItem(AddedDiscInfo disc, IEnumerable<Artist> availableArtists, IEnumerable<Genre> availableGenres)
				: base(disc, availableArtists, availableGenres)
			{
			}

			public override string DiscTypeTitle { get; }

			public override Artist Artist { get; set; }

			public override bool ArtistIsEditable { get; }

			public override bool ArtistIsNotFilled { get; }

			protected override Artist GetSongArtist(AddedSongInfo song)
			{
				return null;
			}
		}

		[Test]
		public void Constructor_IfDiscTitleIsCorrect_InitializesDiscTitleCorrectly()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			//	Act

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Assert

			Assert.AreEqual("Some Title", target.Disc.Title);
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase(" ")]
		public void Constructor_IfDiscTitleIsEmpty_ThrowsInvalidOperationException(string discTitle)
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = discTitle,
			};

			//	Act & Assert

			Assert.Throws<InvalidOperationException>(() => new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>()));
		}

		[TestCase("Broken Crown Halo (CD 1)", "Broken Crown Halo")]
		[TestCase("Broken Crown Halo", "Broken Crown Halo")]
		public void Constructor_InitializesAlbumTitleCorrectly(string discTitle, string expectedAlbumTitle)
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = discTitle,
			};

			//	Act

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Assert

			Assert.AreEqual(expectedAlbumTitle, target.AlbumTitle);
		}

		[Test]
		public void Constructor_InitializesDiscPropertiesCorrectly()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
				UriWithinStorage = new Uri("/SomeDiscUri", UriKind.Relative),
			};

			//	Act

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Assert

			Assert.AreEqual("Some Title", target.Disc.Title);
			Assert.AreEqual("Some Title", target.Disc.AlbumTitle);
			Assert.AreEqual(new Uri("/SomeDiscUri", UriKind.Relative), target.Disc.Uri);
		}

		[Test]
		public void AlbumTitleSetter_UpdatesAlbumTitleCorrectly()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Act

			target.AlbumTitle = "New Album Title";

			//	Assert

			Assert.AreEqual("New Album Title", target.AlbumTitle);
			Assert.AreEqual("New Album Title", target.Disc.AlbumTitle);
		}

		[Test]
		public void AlbumTitleIsEditableGetter_ReturnsTrue()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Act & Assert

			Assert.IsTrue(target.AlbumTitleIsEditable);
		}

		[Test]
		public void WarnAboutUnequalAlbumTitleGetter_WhenAlbumTitleEqualsDiscTitle_ReturnsFalse()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Sanity check
			Assert.AreEqual(target.DiscTitle, target.AlbumTitle);

			//	Act & Assert

			Assert.IsFalse(target.WarnAboutUnequalAlbumTitle);
		}

		[Test]
		public void WarnAboutUnequalAlbumTitleGetter_WhenAlbumTitleDoesNotEqualDiscTitle_ReturnsTrue()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());
			target.AlbumTitle = "Album Title";

			//	Sanity check
			Assert.AreNotEqual(target.DiscTitle, target.AlbumTitle);

			//	Act & Assert

			Assert.IsTrue(target.WarnAboutUnequalAlbumTitle);
		}

		[Test]
		public void YearIsEditableGetter_ReturnsTrue()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Act & Assert

			Assert.IsTrue(target.YearIsEditable);
		}

		[Test]
		public void WarnAboutNotFilledYearGetter_IfYearIsSet_ReturnsFalse()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			target.Year = 2017;

			//	Act & Assert

			Assert.IsFalse(target.WarnAboutNotFilledYear);
		}

		[Test]
		public void WarnAboutNotFilledYearGetter_IfYearIsNotSet_ReturnsTrue()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Act & Assert

			Assert.IsTrue(target.WarnAboutNotFilledYear);
		}


		[Test]
		public void RequiredDataIsFilledGetter_WhenAllRequiredDataIsSet_ReturnsTrue()
		{
			//	Arrange

			Genre genre = new Genre();

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), new[] { genre });
			target.Genre = genre;

			//	Act & Assert

			Assert.IsTrue(target.RequiredDataIsFilled);
		}

		[Test]
		public void RequiredDataIsFilledGetter_WhenGenreIsNotSet_ReturnsFalse()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), new[] { new Genre() });

			//	Act & Assert

			Assert.IsFalse(target.RequiredDataIsFilled);
		}

		[Test]
		public void DiscGetter_WhenCalledMultipleTimes_ReturnsSameDiscObject()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
				UriWithinStorage = new Uri("/SomeDiscUri", UriKind.Relative),
			};

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Act

			var disc1 = target.Disc;
			var disc2 = target.Disc;

			//	Assert

			Assert.AreSame(disc1, disc2);
		}

		[Test]
		public void SongsGetter_FillsSongsDataCorrectly()
		{
			//	Arrange

			var genre = new Genre();

			AddedSongInfo addedSongInfo = new AddedSongInfo("SongSourcePath")
			{
				Artist = "Song Artist",
				Track = 1,
				Title = "Song Title",
			};

			var discInfo = new AddedDiscInfo(new[] { addedSongInfo })
			{
				Title = "Disc Title (CD 1)",
				UriWithinStorage = new Uri("/Some/Disc/Uri", UriKind.Relative),
			};

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), new[] { new Genre() });

			target.Genre = genre;
			target.Year = 2017;

			//	Act

			var songs = target.Songs.ToList();

			//	Assert

			Assert.AreEqual(1, songs.Count);
			var songInfo = songs.Single();
			var song = songInfo.Song;
			var songDisc = song.Disc;
			Assert.AreEqual("Disc Title (CD 1)", songDisc.Title);
			Assert.AreEqual("Disc Title", songDisc.AlbumTitle);
			Assert.AreEqual(new Uri("/Some/Disc/Uri", UriKind.Relative), songDisc.Uri);
			Assert.AreEqual(1, song.TrackNumber);
			Assert.AreEqual(2017, song.Year);
			Assert.AreEqual("Song Title", song.Title);
			Assert.AreSame(genre, song.Genre);
			Assert.IsNull(song.Rating);
			Assert.IsNull(song.LastPlaybackTime);
			Assert.AreEqual(0, song.PlaybacksCount);
			Assert.AreEqual("SongSourcePath", songInfo.SourceFileName);
		}
	}
}

