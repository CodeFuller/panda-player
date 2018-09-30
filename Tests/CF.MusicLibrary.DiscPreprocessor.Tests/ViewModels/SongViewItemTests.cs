using System;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using NUnit.Framework;

namespace CF.MusicLibrary.DiscPreprocessor.Tests.ViewModels
{
	[TestFixture]
	public class SongViewItemTests
	{
		[Test]
		public void ArtistNameSetter_IfArtistIsNotSet_CreatesNewArtistWithSpecifiedName()
		{
			// Arrange

			var addedSong = new AddedSong(new Song(), "SomeSong.mp3");

			var target = new SongViewItem(addedSong);

			// Act

			target.ArtistName = "Some Artist";

			// Assert

			Assert.AreEqual("Some Artist", target.ArtistName);
		}

		[Test]
		public void ArtistNameSetter_IfNewArtistIsSet_CreatesNewArtistWithSpecifiedName()
		{
			// Arrange

			var song = new Song
			{
				Artist = new Artist
				{
					Id = 0,
					Name = "Previous Name",
				}
			};
			var addedSong = new AddedSong(song, "SomeSong.mp3");

			var target = new SongViewItem(addedSong);

			// Act

			target.ArtistName = "New Name";

			// Assert

			Assert.AreEqual("New Name", target.ArtistName);
		}

		[Test]
		public void ArtistNameSetter_IfExistingArtistIsSet_Throws()
		{
			// Arrange

			var song = new Song
			{
				Artist = new Artist
				{
					Id = 12345,
					Name = "Previous Name",
				}
			};
			var addedSong = new AddedSong(song, "SomeSong.mp3");

			var target = new SongViewItem(addedSong);

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.ArtistName = "New Name");
		}
	}
}
