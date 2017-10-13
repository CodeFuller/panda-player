using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CF.Library.Core.Attributes;
using CF.MusicLibrary.Common.DiscArt;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	[TestFixture]
	public class NewDiscViewItemTests
	{
		[ExcludeFromTestCoverage("Empty stub of base abstract class")]
		private class ConcreteDiscViewItem : NewDiscViewItem
		{
			public ConcreteDiscViewItem(IDiscArtImageFile discArtImageFile, AddedDiscInfo disc, IEnumerable<Artist> availableArtists, IEnumerable<Genre> availableGenres)
				: base(discArtImageFile, disc, availableArtists, availableGenres)
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
		public void Constructor_IfDiscArtImageFileArgumentIsNull_ThrowsArgumentNullException()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			//	Act & Assert

			Assert.Throws<ArgumentNullException>(() => new ConcreteDiscViewItem(null, discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>()));
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

			var target = new ConcreteDiscViewItem(Substitute.For<IDiscArtImageFile>(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Assert

			Assert.AreEqual("Some Title", target.AlbumTitle);
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

			Assert.Throws<InvalidOperationException>(() => new ConcreteDiscViewItem(Substitute.For<IDiscArtImageFile>(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>()));
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

			var target = new ConcreteDiscViewItem(Substitute.For<IDiscArtImageFile>(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Assert

			Assert.AreEqual(expectedAlbumTitle, target.AlbumTitle);
		}

		[Test]
		public void AlbumTitleSetter_UpdatesAlbumTitleCorrectly()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem(Substitute.For<IDiscArtImageFile>(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Act

			target.AlbumTitle = "New Album Title";

			//	Assert

			Assert.AreEqual("New Album Title", target.AlbumTitle);
		}

		[Test]
		public void AlbumTitleIsEditableGetter_ReturnsTrue()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem(Substitute.For<IDiscArtImageFile>(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

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

			var target = new ConcreteDiscViewItem(Substitute.For<IDiscArtImageFile>(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

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

			var target = new ConcreteDiscViewItem(Substitute.For<IDiscArtImageFile>(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());
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

			var target = new ConcreteDiscViewItem(Substitute.For<IDiscArtImageFile>(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

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

			var target = new ConcreteDiscViewItem(Substitute.For<IDiscArtImageFile>(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

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

			var target = new ConcreteDiscViewItem(Substitute.For<IDiscArtImageFile>(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

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

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			discArtImageFileStub.ImageIsValid.Returns(true);

			var target = new ConcreteDiscViewItem(discArtImageFileStub, discInfo, Enumerable.Empty<Artist>(), new[] { genre });
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

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			discArtImageFileStub.ImageIsValid.Returns(true);

			var target = new ConcreteDiscViewItem(discArtImageFileStub, discInfo, Enumerable.Empty<Artist>(), new[] { new Genre() });

			//	Act & Assert

			Assert.IsFalse(target.RequiredDataIsFilled);
		}

		[Test]
		public void RequiredDataIsFilledGetter_WhenDiscArtImageIsNotValid_ReturnsFalse()
		{
			//	Arrange

			Genre genre = new Genre();

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			discArtImageFileStub.ImageIsValid.Returns(false);

			var target = new ConcreteDiscViewItem(discArtImageFileStub, discInfo, Enumerable.Empty<Artist>(), new[] { genre });
			target.Genre = genre;

			//	Act & Assert

			Assert.IsFalse(target.RequiredDataIsFilled);
		}

		[Test]
		public void DiscArtIsValidGetter_IfDiscArtIsValid_ReturnsTrue()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			discArtImageFileStub.ImageIsValid.Returns(true);

			var target = new ConcreteDiscViewItem(discArtImageFileStub, discInfo, new[] { new Artist { Name = "Some Artist" } }, new[] { new Genre() });

			//	Act & Assert

			Assert.IsTrue(target.DiscArtIsValid);
		}

		[Test]
		public void DiscArtIsValidGetter_IfDiscArtIsNotValid_ReturnsFalse()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			discArtImageFileStub.ImageIsValid.Returns(false);

			var target = new ConcreteDiscViewItem(discArtImageFileStub, discInfo, new[] { new Artist { Name = "Some Artist" } }, new[] { new Genre() });

			//	Act & Assert

			Assert.IsFalse(target.DiscArtIsValid);
		}

		[Test]
		public void DiscArtInfoGetter_IfDiscArtIsValid_ReturnsImageProperties()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			discArtImageFileStub.ImageIsValid.Returns(true);
			discArtImageFileStub.ImageProperties.Returns("Some Image Properties");

			var target = new ConcreteDiscViewItem(discArtImageFileStub, discInfo, new[] { new Artist { Name = "Some Artist" } }, new[] { new Genre() });

			//	Act & Assert

			Assert.AreEqual("Some Image Properties", target.DiscArtInfo);
		}

		[Test]
		public void DiscArtInfoGetter_IfDiscArtIsNotValid_ReturnsImageStatus()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			discArtImageFileStub.ImageIsValid.Returns(false);
			discArtImageFileStub.ImageStatus.Returns("Some Image Status");

			var target = new ConcreteDiscViewItem(discArtImageFileStub, discInfo, new[] { new Artist { Name = "Some Artist" } }, new[] { new Genre() });

			//	Act & Assert

			Assert.AreEqual("Some Image Status", target.DiscArtInfo);
		}

		[Test]
		public void SetDiscCoverImage_LoadsDiscArtImageFileCorrectly()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			IDiscArtImageFile discArtImageFileMock = Substitute.For<IDiscArtImageFile>();
			var target = new ConcreteDiscViewItem(discArtImageFileMock, discInfo, new[] { new Artist { Name = "Some Artist" } }, new[] { new Genre() });

			//	Act

			target.SetDiscCoverImage("SomeDiscCover.jpg");

			//	Assert

			discArtImageFileMock.Received(1).Load("SomeDiscCover.jpg", false);
		}

		[Test]
		public void UnsetDiscCoverImage_UnloadsDiscArtImageFileCorrectly()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			IDiscArtImageFile discArtImageFileMock = Substitute.For<IDiscArtImageFile>();
			var target = new ConcreteDiscViewItem(discArtImageFileMock, discInfo, new[] { new Artist { Name = "Some Artist" } }, new[] { new Genre() });

			//	Act

			target.UnsetDiscCoverImage();

			//	Assert

			discArtImageFileMock.Received(1).Unload();
		}

		[Test]
		public void DiscArtImageIsValidPropertyChangedHandler_RaisesPropertyChangedEventForAllAffectedProperties()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			var target = new ConcreteDiscViewItem(discArtImageFileStub, discInfo, new[] { new Artist { Name = "Some Artist" } }, new[] { new Genre() });

			var changedProperties = new List<string>();
			target.PropertyChanged += (sender, e) => changedProperties.Add(e.PropertyName);

			//	Act

			discArtImageFileStub.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(new PropertyChangedEventArgs(nameof(IDiscArtImageFile.ImageIsValid)));

			//	Assert

			CollectionAssert.Contains(changedProperties, nameof(DiscViewItem.DiscArtIsValid));
			CollectionAssert.Contains(changedProperties, nameof(DiscViewItem.DiscArtInfo));
			CollectionAssert.Contains(changedProperties, nameof(DiscViewItem.RequiredDataIsFilled));
		}

		[Test]
		public void DiscArtImageStatusPropertyChangedHandler_RaisesPropertyChangedEventForDiscArtInfo()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			var target = new ConcreteDiscViewItem(discArtImageFileStub, discInfo, new[] { new Artist { Name = "Some Artist" } }, new[] { new Genre() });

			var changedProperties = new List<string>();
			target.PropertyChanged += (sender, e) => changedProperties.Add(e.PropertyName);

			//	Act

			discArtImageFileStub.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(new PropertyChangedEventArgs(nameof(IDiscArtImageFile.ImageStatus)));

			//	Assert

			CollectionAssert.Contains(changedProperties, nameof(DiscViewItem.DiscArtInfo));
		}

		[Test]
		public void DiscArtImagePropertiesPropertyChangedHandler_RaisesPropertyChangedEventForDiscArtInfo()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			var target = new ConcreteDiscViewItem(discArtImageFileStub, discInfo, new[] { new Artist { Name = "Some Artist" } }, new[] { new Genre() });

			var changedProperties = new List<string>();
			target.PropertyChanged += (sender, e) => changedProperties.Add(e.PropertyName);

			//	Act

			discArtImageFileStub.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(new PropertyChangedEventArgs(nameof(IDiscArtImageFile.ImageProperties)));

			//	Assert

			CollectionAssert.Contains(changedProperties, nameof(DiscViewItem.DiscArtInfo));
		}

		[Test]
		public void AddedDiscCoverImageGetter_IfDiscArtIsNotValid_ThrowsInvalidOperationException()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			discArtImageFileStub.ImageIsValid.Returns(false);
			var target = new ConcreteDiscViewItem(discArtImageFileStub, discInfo, new[] { new Artist { Name = "Some Artist" } }, new[] { new Genre() });

			//	Act & Assert

			AddedDiscCoverImage addedDiscCoverImage = null;
			Assert.Throws<InvalidOperationException>(() => addedDiscCoverImage = target.AddedDiscCoverImage);
		}

		[Test]
		public void AddedDiscCoverImageGetter_IfDiscArtIsValid_ReturnsCorrectAddedDiscCoverImage()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
				UriWithinStorage = new Uri("/Some/Disc/Uri", UriKind.Relative),
			};

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			discArtImageFileStub.ImageIsValid.Returns(true);
			discArtImageFileStub.ImageFileName.Returns("SomeDiscCover.jpg");
			var target = new ConcreteDiscViewItem(discArtImageFileStub, discInfo, new[] { new Artist { Name = "Some Artist" } }, new[] { new Genre() });

			//	Act

			AddedDiscCoverImage addedDiscCoverImage = target.AddedDiscCoverImage;

			//	Assert

			Assert.AreEqual("SomeDiscCover.jpg", addedDiscCoverImage.CoverImageFileName);
			Assert.AreEqual(new Uri("/Some/Disc/Uri", UriKind.Relative), addedDiscCoverImage.Disc.Uri);
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

			var target = new ConcreteDiscViewItem(Substitute.For<IDiscArtImageFile>(), discInfo, Enumerable.Empty<Artist>(), new[] { new Genre() });

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

