using System;
using System.Threading;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.LibraryChecker.Checkers;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.LibraryChecker.Tests
{
	[TestFixture]
	public class ApplicationLogicTests
	{
		[TestCase("--check", LibraryCheckFlags.CheckDiscsConsistency | LibraryCheckFlags.CheckLibraryStorage)]
		[TestCase("--check-discs", LibraryCheckFlags.CheckDiscsConsistency)]
		[TestCase("--check-storage", LibraryCheckFlags.CheckLibraryStorage)]
		[TestCase("--check-checksums", LibraryCheckFlags.CheckChecksums)]
		[TestCase("--check-tags", LibraryCheckFlags.CheckTagData)]
		[TestCase("--check-images", LibraryCheckFlags.CheckImages)]
		[TestCase("--check-artists", LibraryCheckFlags.CheckArtistsOnLastFm)]
		[TestCase("--check-albums", LibraryCheckFlags.CheckAlbumsOnLastFm)]
		[TestCase("--check-songs", LibraryCheckFlags.CheckSongsOnLastFm)]
		public void Run_ForSpecificCheckCommand_InvokesLibraryCheckerCorrectly(string command, LibraryCheckFlags expectedCheckFlags)
		{
			// Arrange

			var libraryCheckerMock = Substitute.For<ILibraryConsistencyChecker>();

			var target = new ApplicationLogic(libraryCheckerMock, Substitute.For<IUriCheckScope>());

			// Act

			target.Run(new[] { command }, CancellationToken.None).Wait();

			// Assert

			libraryCheckerMock.Received(1).CheckLibrary(expectedCheckFlags, false, Arg.Any<CancellationToken>());
		}

		[Test]
		public void Run_ForMultipleCheckCommands_InvokesLibraryCheckerCorrectly()
		{
			// Arrange

			var libraryCheckerMock = Substitute.For<ILibraryConsistencyChecker>();

			var target = new ApplicationLogic(libraryCheckerMock, Substitute.For<IUriCheckScope>());

			// Act

			target.Run(new[] { "--check-storage", "--check-images" }, CancellationToken.None).Wait();

			// Assert

			libraryCheckerMock.Received(1).CheckLibrary(LibraryCheckFlags.CheckLibraryStorage | LibraryCheckFlags.CheckImages, false, Arg.Any<CancellationToken>());
		}

		[Test]
		public void Run_ForCheckCommandIfFixSwitchIsSpecified_InvokesLibraryCheckerInFixMode()
		{
			// Arrange

			var libraryCheckerMock = Substitute.For<ILibraryConsistencyChecker>();

			var target = new ApplicationLogic(libraryCheckerMock, Substitute.For<IUriCheckScope>());

			// Act

			target.Run(new[] { "--check-discs", "--fix" }, CancellationToken.None).Wait();

			// Assert

			libraryCheckerMock.Received(1).CheckLibrary(LibraryCheckFlags.CheckDiscsConsistency, true, Arg.Any<CancellationToken>());
		}

		[Test]
		public void Run_ForUnifyTagsCommand_InvokesLibraryCheckerInFixMode()
		{
			// Arrange

			var libraryCheckerMock = Substitute.For<ILibraryConsistencyChecker>();

			var target = new ApplicationLogic(libraryCheckerMock, Substitute.For<IUriCheckScope>());

			// Act

			target.Run(new[] { "--unify-tags" }, CancellationToken.None).Wait();

			// Assert

			libraryCheckerMock.Received(1).UnifyTags(Arg.Any<CancellationToken>());
		}

		[TestCase("--check-discs")]
		[TestCase("--unify-tags")]
		public void Run_IfScopeIsSpecified_SetsScopeCorrectly(string command)
		{
			// Arrange

			var checkScopeMock = Substitute.For<IUriCheckScope>();

			var target = new ApplicationLogic(Substitute.For<ILibraryConsistencyChecker>(), checkScopeMock);

			// Act

			target.Run(new[] { command, "--scope=/Some/Scope" }, CancellationToken.None).Wait();

			// Assert

			checkScopeMock.Received(1).SetScopeUri(new Uri("/Some/Scope", UriKind.Relative));
		}

		[TestCase("--check-discs")]
		[TestCase("--unify-tags")]
		public void Run_IfScopeIsNotSpecified_UsesDefaultScope(string command)
		{
			// Arrange

			var checkScopeMock = Substitute.For<IUriCheckScope>();

			var target = new ApplicationLogic(Substitute.For<ILibraryConsistencyChecker>(), checkScopeMock);

			// Act

			target.Run(new[] { command }, CancellationToken.None).Wait();

			// Assert

			checkScopeMock.Received(1).SetScopeUri(new Uri("/", UriKind.Relative));
		}

		public void Run_IfNoCommandIsSpecified_ReturnsWithNoAction()
		{
			// Arrange

			var libraryCheckerMock = Substitute.For<ILibraryConsistencyChecker>();

			var target = new ApplicationLogic(libraryCheckerMock, Substitute.For<IUriCheckScope>());

			// Act

			var result = target.Run(new[] { "--fix", "--scope=/Some/Scope" }, CancellationToken.None).Result;

			// Assert

			Assert.AreEqual(1, result);
			libraryCheckerMock.DidNotReceive().CheckLibrary(Arg.Any<LibraryCheckFlags>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());
			libraryCheckerMock.DidNotReceive().UnifyTags(Arg.Any<CancellationToken>());
		}
	}
}
