using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PandaPlayer.DiscAdder.Internal;
using PandaPlayer.DiscAdder.UnitTests.TestHelpers;
using PandaPlayer.DiscAdder.ViewModels.Interfaces;
using PandaPlayer.DiscAdder.ViewModels.SourceContent;

namespace PandaPlayer.DiscAdder.UnitTests.Internal
{
	[TestClass]
	public class SourceContentCheckerTests
	{
		[TestMethod]
		public void SetContentCorrectness_IfContentMatch_ClearsContentIsIncorrectPropertyForDiscsAndSongs()
		{
			// Arrange

			var referenceDiscs = new[]
			{
				new ReferenceDiscContent("Disc 1", new[]
				{
					new ReferenceSongContent(1, "Song 1"),
					new ReferenceSongContent(2, "Song 2"),
				}).ToReferenceDiscTreeItem(),

				new ReferenceDiscContent("Disc 2", new[]
				{
					new ReferenceSongContent(1, "Song 3"),
				}).ToReferenceDiscTreeItem(),
			};

			var actualDiscs = new[]
			{
				new ActualDiscContent("Disc 1", new[]
				{
					new ActualSongContent("01 - Song 1.mp3"),
					new ActualSongContent("02 - Song 2.mp3"),
				}).ToActualDiscTreeItem(),

				new ActualDiscContent("Disc 2", new[]
				{
					new ActualSongContent("01 - Song 3.mp3"),
				}).ToActualDiscTreeItem(),
			};

			var referenceContentStub = CreateReferenceContentStub(referenceDiscs);
			var actualContentStub = CreateActualContentStub(actualDiscs);

			var target = new SourceContentChecker();

			// Act

			target.SetContentCorrectness(referenceContentStub, actualContentStub);

			// Assert

			referenceDiscs[0].ContentIsIncorrect.Should().BeFalse();
			referenceDiscs[0].ExpectedSongs.Should().OnlyContain(x => !x.ContentIsIncorrect);
			referenceDiscs[1].ContentIsIncorrect.Should().BeFalse();
			referenceDiscs[1].ExpectedSongs.Should().OnlyContain(x => !x.ContentIsIncorrect);

			actualDiscs[0].ContentIsIncorrect.Should().BeFalse();
			actualDiscs[0].Songs.Should().OnlyContain(x => !x.ContentIsIncorrect);
			actualDiscs[1].ContentIsIncorrect.Should().BeFalse();
			actualDiscs[1].Songs.Should().OnlyContain(x => !x.ContentIsIncorrect);
		}

		[TestMethod]
		public void SetContentCorrectness_IfNumberOfReferenceDiscsIsSmaller_SetsRestActualDiscsAsIncorrect()
		{
			// Arrange

			var referenceDiscs = new[]
			{
				new ReferenceDiscContent("Disc 1", new[] { new ReferenceSongContent(1, "Song 1") }).ToReferenceDiscTreeItem(),
			};

			var actualDiscs = new[]
			{
				new ActualDiscContent("Disc 1", new[] { new ActualSongContent("01 - Song 1.mp3") }).ToActualDiscTreeItem(),
				new ActualDiscContent("Disc 2", new[] { new ActualSongContent("01 - Song 2.mp3") }).ToActualDiscTreeItem(),
				new ActualDiscContent("Disc 3", new[] { new ActualSongContent("01 - Song 3.mp3") }).ToActualDiscTreeItem(),
			};

			var referenceContentStub = CreateReferenceContentStub(referenceDiscs);
			var actualContentStub = CreateActualContentStub(actualDiscs);

			var target = new SourceContentChecker();

			// Act

			target.SetContentCorrectness(referenceContentStub, actualContentStub);

			// Assert

			referenceDiscs[0].ContentIsIncorrect.Should().BeFalse();
			referenceDiscs[0].ExpectedSongs.Should().OnlyContain(x => !x.ContentIsIncorrect);

			actualDiscs[0].ContentIsIncorrect.Should().BeFalse();
			actualDiscs[0].Songs.Should().OnlyContain(x => !x.ContentIsIncorrect);

			actualDiscs[1].ContentIsIncorrect.Should().BeTrue();
			actualDiscs[1].Songs.Should().OnlyContain(x => x.ContentIsIncorrect);

			actualDiscs[2].ContentIsIncorrect.Should().BeTrue();
			actualDiscs[2].Songs.Should().OnlyContain(x => x.ContentIsIncorrect);
		}

		[TestMethod]
		public void SetContentCorrectness_IfNumberOfActualDiscsIsSmaller_SetsRestReferenceDiscsAsIncorrect()
		{
			// Arrange

			var referenceDiscs = new[]
			{
				new ReferenceDiscContent("Disc 1", new[] { new ReferenceSongContent(1, "Song 1") }).ToReferenceDiscTreeItem(),
				new ReferenceDiscContent("Disc 2", new[] { new ReferenceSongContent(1, "Song 2") }).ToReferenceDiscTreeItem(),
				new ReferenceDiscContent("Disc 3", new[] { new ReferenceSongContent(1, "Song 3") }).ToReferenceDiscTreeItem(),
			};

			var actualDiscs = new[]
			{
				new ActualDiscContent("Disc 1", new[] { new ActualSongContent("01 - Song 1.mp3") }).ToActualDiscTreeItem(),
			};

			var referenceContentStub = CreateReferenceContentStub(referenceDiscs);
			var actualContentStub = CreateActualContentStub(actualDiscs);

			var target = new SourceContentChecker();

			// Act

			target.SetContentCorrectness(referenceContentStub, actualContentStub);

			// Assert

			referenceDiscs[0].ContentIsIncorrect.Should().BeFalse();
			referenceDiscs[0].ExpectedSongs.Should().OnlyContain(x => !x.ContentIsIncorrect);

			referenceDiscs[1].ContentIsIncorrect.Should().BeTrue();
			referenceDiscs[1].ExpectedSongs.Should().OnlyContain(x => x.ContentIsIncorrect);

			referenceDiscs[2].ContentIsIncorrect.Should().BeTrue();
			referenceDiscs[2].ExpectedSongs.Should().OnlyContain(x => x.ContentIsIncorrect);

			actualDiscs[0].ContentIsIncorrect.Should().BeFalse();
			actualDiscs[0].Songs.Should().OnlyContain(x => !x.ContentIsIncorrect);
		}

		[TestMethod]
		public void SetContentCorrectness_IfNumberOfReferenceSongsIsSmaller_SetsRestActualSongsAsIncorrect()
		{
			// Arrange

			var referenceDiscs = new[]
			{
				new ReferenceDiscContent("Disc 1", new[]
				{
					new ReferenceSongContent(1, "Song 1"),
				}).ToReferenceDiscTreeItem(),
			};

			var actualDiscs = new[]
			{
				new ActualDiscContent("Disc 1", new[]
				{
					new ActualSongContent("01 - Song 1.mp3"),
					new ActualSongContent("02 - Song 2.mp3"),
					new ActualSongContent("03 - Song 3.mp3"),
				}).ToActualDiscTreeItem(),
			};

			var referenceContentStub = CreateReferenceContentStub(referenceDiscs);
			var actualContentStub = CreateActualContentStub(actualDiscs);

			var target = new SourceContentChecker();

			// Act

			target.SetContentCorrectness(referenceContentStub, actualContentStub);

			// Assert

			referenceDiscs[0].ContentIsIncorrect.Should().BeTrue();
			referenceDiscs[0].ExpectedSongs.Should().OnlyContain(x => !x.ContentIsIncorrect);

			actualDiscs[0].ContentIsIncorrect.Should().BeTrue();

			var actualSongs = actualDiscs[0].Songs.ToList();
			actualSongs[0].ContentIsIncorrect.Should().BeFalse();
			actualSongs[1].ContentIsIncorrect.Should().BeTrue();
			actualSongs[2].ContentIsIncorrect.Should().BeTrue();
		}

		[TestMethod]
		public void SetContentCorrectness_IfNumberOfActualSongsIsSmaller_SetsRestReferenceSongsAsIncorrect()
		{
			// Arrange

			var referenceDiscs = new[]
			{
				new ReferenceDiscContent("Disc 1", new[]
				{
					new ReferenceSongContent(1, "Song 1"),
					new ReferenceSongContent(2, "Song 2"),
					new ReferenceSongContent(3, "Song 3"),
				}).ToReferenceDiscTreeItem(),
			};

			var actualDiscs = new[]
			{
				new ActualDiscContent("Disc 1", new[]
				{
					new ActualSongContent("01 - Song 1.mp3"),
				}).ToActualDiscTreeItem(),
			};

			var referenceContentStub = CreateReferenceContentStub(referenceDiscs);
			var actualContentStub = CreateActualContentStub(actualDiscs);

			var target = new SourceContentChecker();

			// Act

			target.SetContentCorrectness(referenceContentStub, actualContentStub);

			// Assert

			referenceDiscs[0].ContentIsIncorrect.Should().BeTrue();

			var referenceSongs = referenceDiscs[0].ExpectedSongs.ToList();
			referenceSongs[0].ContentIsIncorrect.Should().BeFalse();
			referenceSongs[1].ContentIsIncorrect.Should().BeTrue();
			referenceSongs[2].ContentIsIncorrect.Should().BeTrue();

			actualDiscs[0].ContentIsIncorrect.Should().BeTrue();
			actualDiscs[0].Songs.Should().OnlyContain(x => !x.ContentIsIncorrect);
		}

		[TestMethod]
		public void SetContentCorrectness_IfSongTitlesDoNotMatch_SetsSongAndDiscAsIncorrect()
		{
			// Arrange

			var referenceDiscs = new[]
			{
				new ReferenceDiscContent("Disc 1", new[]
				{
					new ReferenceSongContent(1, "Song 1"),
					new ReferenceSongContent(2, "Song 2"),
					new ReferenceSongContent(3, "Song 3"),
				}).ToReferenceDiscTreeItem(),
			};

			var actualDiscs = new[]
			{
				new ActualDiscContent("Disc 1", new[]
				{
					new ActualSongContent("01 - Song 1.mp3"),
					new ActualSongContent("02 - Song 123.mp3"),
					new ActualSongContent("03 - Song 3.mp3"),
				}).ToActualDiscTreeItem(),
			};

			var referenceContentStub = CreateReferenceContentStub(referenceDiscs);
			var actualContentStub = CreateActualContentStub(actualDiscs);

			var target = new SourceContentChecker();

			// Act

			target.SetContentCorrectness(referenceContentStub, actualContentStub);

			// Assert

			referenceDiscs[0].ContentIsIncorrect.Should().BeTrue();

			var referenceSongs = referenceDiscs[0].ExpectedSongs.ToList();
			referenceSongs[0].ContentIsIncorrect.Should().BeFalse();
			referenceSongs[1].ContentIsIncorrect.Should().BeTrue();
			referenceSongs[2].ContentIsIncorrect.Should().BeFalse();

			actualDiscs[0].ContentIsIncorrect.Should().BeTrue();

			var actualSongs = actualDiscs[0].Songs.ToList();
			actualSongs[0].ContentIsIncorrect.Should().BeFalse();
			actualSongs[1].ContentIsIncorrect.Should().BeTrue();
			actualSongs[2].ContentIsIncorrect.Should().BeFalse();
		}

		[TestMethod]
		public void SetContentCorrectness_IfSongTrackNumbersDoNotMatch_SetsSongAndDiscAsIncorrect()
		{
			// Arrange

			var referenceDiscs = new[]
			{
				new ReferenceDiscContent("Disc 1", new[]
				{
					new ReferenceSongContent(1, "Song 1"),
					new ReferenceSongContent(2, "Song 2"),
					new ReferenceSongContent(3, "Song 3"),
				}).ToReferenceDiscTreeItem(),
			};

			var actualDiscs = new[]
			{
				new ActualDiscContent("Disc 1", new[]
				{
					new ActualSongContent("01 - Song 1.mp3"),
					new ActualSongContent("04 - Song 2.mp3"),
					new ActualSongContent("03 - Song 3.mp3"),
				}).ToActualDiscTreeItem(),
			};

			var referenceContentStub = CreateReferenceContentStub(referenceDiscs);
			var actualContentStub = CreateActualContentStub(actualDiscs);

			var target = new SourceContentChecker();

			// Act

			target.SetContentCorrectness(referenceContentStub, actualContentStub);

			// Assert

			referenceDiscs[0].ContentIsIncorrect.Should().BeTrue();

			var referenceSongs = referenceDiscs[0].ExpectedSongs.ToList();
			referenceSongs[0].ContentIsIncorrect.Should().BeFalse();
			referenceSongs[1].ContentIsIncorrect.Should().BeTrue();
			referenceSongs[2].ContentIsIncorrect.Should().BeFalse();

			actualDiscs[0].ContentIsIncorrect.Should().BeTrue();

			var actualSongs = actualDiscs[0].Songs.ToList();
			actualSongs[0].ContentIsIncorrect.Should().BeFalse();
			actualSongs[1].ContentIsIncorrect.Should().BeTrue();
			actualSongs[2].ContentIsIncorrect.Should().BeFalse();
		}

		[TestMethod]
		public void SetContentCorrectness_IfTitlesMatchForSongsWithoutTrackNumbers_ClearsContentIsIncorrectPropertyForDiscsAndSongs()
		{
			// Arrange

			var referenceDiscs = new[]
			{
				new ReferenceDiscContent("Disc 1", new[]
				{
					new ReferenceSongContent(1, "Song 1"),
					new ReferenceSongContent(2, "Song 2"),
				}).ToReferenceDiscTreeItem(),
			};

			var actualDiscs = new[]
			{
				new ActualDiscContent("Disc 1", new[]
				{
					new ActualSongContent("Song 1.mp3"),
					new ActualSongContent("Song 2.mp3"),
				}).ToActualDiscTreeItem(),
			};

			var referenceContentStub = CreateReferenceContentStub(referenceDiscs);
			var actualContentStub = CreateActualContentStub(actualDiscs);

			var target = new SourceContentChecker();

			// Act

			target.SetContentCorrectness(referenceContentStub, actualContentStub);

			// Assert

			referenceDiscs[0].ContentIsIncorrect.Should().BeFalse();
			referenceDiscs[0].ExpectedSongs.Should().OnlyContain(x => !x.ContentIsIncorrect);

			actualDiscs[0].ContentIsIncorrect.Should().BeFalse();
			actualDiscs[0].Songs.Should().OnlyContain(x => !x.ContentIsIncorrect);
		}

		[TestMethod]
		public void SetContentCorrectness_IfTitlesDoNotMatchForSongsWithoutTrackNumbers_SetsSongAndDiscAsIncorrect()
		{
			// Arrange

			var referenceDiscs = new[]
			{
				new ReferenceDiscContent("Disc 1", new[]
				{
					new ReferenceSongContent(1, "Song 1"),
					new ReferenceSongContent(2, "Song 2"),
					new ReferenceSongContent(3, "Song 3"),
				}).ToReferenceDiscTreeItem(),
			};

			var actualDiscs = new[]
			{
				new ActualDiscContent("Disc 1", new[]
				{
					new ActualSongContent("Song 1.mp3"),
					new ActualSongContent("Song 123.mp3"),
					new ActualSongContent("Song 3.mp3"),
				}).ToActualDiscTreeItem(),
			};

			var referenceContentStub = CreateReferenceContentStub(referenceDiscs);
			var actualContentStub = CreateActualContentStub(actualDiscs);

			var target = new SourceContentChecker();

			// Act

			target.SetContentCorrectness(referenceContentStub, actualContentStub);

			// Assert

			referenceDiscs[0].ContentIsIncorrect.Should().BeTrue();

			var referenceSongs = referenceDiscs[0].ExpectedSongs.ToList();
			referenceSongs[0].ContentIsIncorrect.Should().BeFalse();
			referenceSongs[1].ContentIsIncorrect.Should().BeTrue();
			referenceSongs[2].ContentIsIncorrect.Should().BeFalse();

			actualDiscs[0].ContentIsIncorrect.Should().BeTrue();

			var actualSongs = actualDiscs[0].Songs.ToList();
			actualSongs[0].ContentIsIncorrect.Should().BeFalse();
			actualSongs[1].ContentIsIncorrect.Should().BeTrue();
			actualSongs[2].ContentIsIncorrect.Should().BeFalse();
		}

		[TestMethod]
		public void SetContentCorrectness_IfTitleWithDotCharacterMatches_ClearsContentIsIncorrectPropertyForDiscsAndSongs()
		{
			// Arrange

			var referenceDiscs = new[]
			{
				new ReferenceDiscContent("Disc 1", new[] { new ReferenceSongContent(1, "Song 1 (feat. Eminem)") }).ToReferenceDiscTreeItem(),
				new ReferenceDiscContent("Disc 2", new[] { new ReferenceSongContent(1, "Song 2 (feat. Eminem)") }).ToReferenceDiscTreeItem(),
			};

			var actualDiscs = new[]
			{
				new ActualDiscContent("Disc 1", new[] { new ActualSongContent("01 - Song 1 (feat. Eminem).mp3") }).ToActualDiscTreeItem(),
				new ActualDiscContent("Disc 2", new[] { new ActualSongContent("Song 2 (feat. Eminem).mp3") }).ToActualDiscTreeItem(),
			};

			var referenceContentStub = CreateReferenceContentStub(referenceDiscs);
			var actualContentStub = CreateActualContentStub(actualDiscs);

			var target = new SourceContentChecker();

			// Act

			target.SetContentCorrectness(referenceContentStub, actualContentStub);

			// Assert

			referenceDiscs[0].ContentIsIncorrect.Should().BeFalse();
			referenceDiscs[0].ExpectedSongs.Should().OnlyContain(x => !x.ContentIsIncorrect);
			referenceDiscs[1].ContentIsIncorrect.Should().BeFalse();
			referenceDiscs[1].ExpectedSongs.Should().OnlyContain(x => !x.ContentIsIncorrect);

			actualDiscs[0].ContentIsIncorrect.Should().BeFalse();
			actualDiscs[0].Songs.Should().OnlyContain(x => !x.ContentIsIncorrect);
			actualDiscs[1].ContentIsIncorrect.Should().BeFalse();
			actualDiscs[1].Songs.Should().OnlyContain(x => !x.ContentIsIncorrect);
		}

		[TestMethod]
		public void SetContentCorrectness_IfTitleWithDotCharacterDoesNotMatch_SetsSongAndDiscAsIncorrect()
		{
			// Arrange

			var referenceDiscs = new[]
			{
				new ReferenceDiscContent("Disc 1", new[] { new ReferenceSongContent(1, "Song 1 (feat. Eminem)") }).ToReferenceDiscTreeItem(),
				new ReferenceDiscContent("Disc 2", new[] { new ReferenceSongContent(1, "Song 2 (feat. Eminem)") }).ToReferenceDiscTreeItem(),
			};

			var actualDiscs = new[]
			{
				new ActualDiscContent("Disc 1", new[] { new ActualSongContent("01 - Song 123 (feat. Eminem).mp3") }).ToActualDiscTreeItem(),
				new ActualDiscContent("Disc 2", new[] { new ActualSongContent("Song 234 (feat. Eminem).mp3") }).ToActualDiscTreeItem(),
			};

			var referenceContentStub = CreateReferenceContentStub(referenceDiscs);
			var actualContentStub = CreateActualContentStub(actualDiscs);

			var target = new SourceContentChecker();

			// Act

			target.SetContentCorrectness(referenceContentStub, actualContentStub);

			// Assert

			referenceDiscs[0].ContentIsIncorrect.Should().BeTrue();
			referenceDiscs[0].ExpectedSongs.Should().OnlyContain(x => x.ContentIsIncorrect);
			referenceDiscs[1].ContentIsIncorrect.Should().BeTrue();
			referenceDiscs[1].ExpectedSongs.Should().OnlyContain(x => x.ContentIsIncorrect);

			actualDiscs[0].ContentIsIncorrect.Should().BeTrue();
			actualDiscs[0].Songs.Should().OnlyContain(x => x.ContentIsIncorrect);
			actualDiscs[1].ContentIsIncorrect.Should().BeTrue();
			actualDiscs[1].Songs.Should().OnlyContain(x => x.ContentIsIncorrect);
		}

		private static IReferenceContentViewModel CreateReferenceContentStub(IEnumerable<ReferenceDiscTreeItem> referenceDiscs)
		{
			var referenceContentStub = new Mock<IReferenceContentViewModel>();
			referenceContentStub.Setup(x => x.ExpectedDiscs).Returns(new ObservableCollection<ReferenceDiscTreeItem>(referenceDiscs));

			return referenceContentStub.Object;
		}

		private static IActualContentViewModel CreateActualContentStub(IEnumerable<ActualDiscTreeItem> actualDiscs)
		{
			var actualContentStub = new Mock<IActualContentViewModel>();
			actualContentStub.Setup(x => x.Discs).Returns(new ObservableCollection<ActualDiscTreeItem>(actualDiscs));

			return actualContentStub.Object;
		}
	}
}
