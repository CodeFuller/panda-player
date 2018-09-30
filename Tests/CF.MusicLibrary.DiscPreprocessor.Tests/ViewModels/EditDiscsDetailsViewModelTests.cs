using System;
using System.Linq;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using CF.MusicLibrary.DiscPreprocessor.ViewModels;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.DiscPreprocessor.Tests.ViewModels
{
	[TestFixture]
	public class EditDiscsDetailsViewModelTests
	{
		[Test]
		public void AddedDiscs_ForNewDiscs_ReturnsDiscsWithCorrectInfo()
		{
			// Arrange

			var addedDiscInfo = new AddedDiscInfo(Array.Empty<AddedSongInfo>())
			{
				DiscTitle = "Some Disc",
				SourcePath = @"Workshop\Some Artist\2000 - Some Disc",
				DiscType = DsicType.ArtistDisc,
				Artist = "Some Artist",
			};

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>());
			target.SetDiscs(new[] { addedDiscInfo }).Wait();

			// Act

			var addedDiscs = target.AddedDiscs.ToList();

			// Assert

			Assert.AreEqual(1, addedDiscs.Count);
			var addedDisc = addedDiscs.Single();
			Assert.AreSame(target.Discs[0].Disc, addedDisc.Disc);
			Assert.IsTrue(addedDisc.IsNewDisc);
			Assert.AreEqual(@"Workshop\Some Artist\2000 - Some Disc", addedDisc.SourcePath);
		}

		[Test]
		public void AddedDiscs_ForExistingDiscs_ReturnsDiscsWithCorrectInfo()
		{
			// Arrange

			var existingDisc = new Disc
			{
				Uri = new Uri("/Some Artist/2000 - Some Disc", UriKind.Relative),
				SongsUnordered = new[] { new Song() },
			};

			var addedDiscInfo = new AddedDiscInfo(Array.Empty<AddedSongInfo>())
			{
				DiscTitle = "Some Disc",
				SourcePath = @"Workshop\Some Artist\2000 - Some Disc",
				UriWithinStorage = new Uri("/Some Artist/2000 - Some Disc", UriKind.Relative),
				DiscType = DsicType.ArtistDisc,
				Artist = "Some Artist",
			};

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Repeat(existingDisc, 1)));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>());
			target.SetDiscs(new[] { addedDiscInfo }).Wait();

			// Sanity check
			Assert.IsTrue(target.Discs.Single() is ExistingDiscViewItem);

			// Act

			var addedDiscs = target.AddedDiscs.ToList();

			// Assert

			Assert.AreEqual(1, addedDiscs.Count);
			var addedDisc = addedDiscs.Single();
			Assert.AreSame(target.Discs[0].Disc, addedDisc.Disc);
			Assert.IsFalse(addedDisc.IsNewDisc);
			Assert.AreEqual(@"Workshop\Some Artist\2000 - Some Disc", addedDisc.SourcePath);
		}
	}
}
