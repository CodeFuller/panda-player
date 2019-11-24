using System;
using System.Threading;
using CF.Library.Core.Facades;
using CF.MusicLibrary.LibraryToolkit.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.LibraryToolkit.Tests
{
	[TestFixture]
	public class ApplicationLogicTests
	{
		[Test]
		public void Run_ForMigrateDatabaseCommandWithCorrectArguments_InvokesMigrateDatabaseCommand()
		{
			// Arrange

			var migrateDatabaseCommandMock = Substitute.For<IMigrateDatabaseCommand>();
			var seedApiDatabaseCommandMock = Substitute.For<ISeedApiDatabaseCommand>();

			var commandLine = new[] { "--migrate-database", "SourceDbFile.db", "TargetDbFile.db", };

			var target = new ApplicationLogic(migrateDatabaseCommandMock, seedApiDatabaseCommandMock, Substitute.For<IFileSystemFacade>());

			// Act

			var exitCode = target.Run(commandLine, CancellationToken.None).Result;

			// Assert

			Assert.AreEqual(0, exitCode);
			migrateDatabaseCommandMock.Received(1).Execute("SourceDbFile.db", "TargetDbFile.db", CancellationToken.None);
			seedApiDatabaseCommandMock.DidNotReceive().Execute(Arg.Any<CancellationToken>());
		}

		[Test]
		public void Run_ForMigrateDatabaseCommandWithIncorrectArguments_ReturnsWithoutExecutingAnyCommand()
		{
			// Arrange

			var migrateDatabaseCommandMock = Substitute.For<IMigrateDatabaseCommand>();
			var seedApiDatabaseCommandMock = Substitute.For<ISeedApiDatabaseCommand>();

			var commandLine = new[] { "--migrate-database", "SourceDbFile.db", "TargetDbFile.db", "Some extra argument" };

			var target = new ApplicationLogic(migrateDatabaseCommandMock, seedApiDatabaseCommandMock, Substitute.For<IFileSystemFacade>());

			// Act

			var exitCode = target.Run(commandLine, CancellationToken.None).Result;

			// Assert

			Assert.AreEqual(1, exitCode);
			migrateDatabaseCommandMock.DidNotReceive().Execute(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
			seedApiDatabaseCommandMock.DidNotReceive().Execute(Arg.Any<CancellationToken>());
		}

		[Test]
		public void Run_ForSeedApiDatabaseCommandWithCorrectArguments_InvokesSeedApiDatabaseCommand()
		{
			// Arrange

			var migrateDatabaseCommandMock = Substitute.For<IMigrateDatabaseCommand>();
			var seedApiDatabaseCommandMock = Substitute.For<ISeedApiDatabaseCommand>();

			var commandLine = new[] { "--seed-api-database" };

			var target = new ApplicationLogic(migrateDatabaseCommandMock, seedApiDatabaseCommandMock, Substitute.For<IFileSystemFacade>());

			// Act

			var exitCode = target.Run(commandLine, CancellationToken.None).Result;

			// Assert

			Assert.AreEqual(0, exitCode);
			migrateDatabaseCommandMock.DidNotReceive().Execute(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
			seedApiDatabaseCommandMock.Received(1).Execute(CancellationToken.None);
		}

		[Test]
		public void Run_ForSeedApiDatabaseCommandWithIncorrectArguments_ReturnsWithoutExecutingAnyCommand()
		{
			// Arrange

			var migrateDatabaseCommandMock = Substitute.For<IMigrateDatabaseCommand>();
			var seedApiDatabaseCommandMock = Substitute.For<ISeedApiDatabaseCommand>();

			var commandLine = new[] { "--seed-api-database", "SourceDbFile.db", };

			var target = new ApplicationLogic(migrateDatabaseCommandMock, seedApiDatabaseCommandMock, Substitute.For<IFileSystemFacade>());

			// Act

			var exitCode = target.Run(commandLine, CancellationToken.None).Result;

			// Assert

			Assert.AreEqual(1, exitCode);
			migrateDatabaseCommandMock.DidNotReceive().Execute(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
			seedApiDatabaseCommandMock.DidNotReceive().Execute(Arg.Any<CancellationToken>());
		}

		[Test]
		public void Run_IfNoCommandsAreSpecified_ReturnsWithoutExecutingAnyCommand()
		{
			// Arrange

			var migrateDatabaseCommandMock = Substitute.For<IMigrateDatabaseCommand>();
			var seedApiDatabaseCommandMock = Substitute.For<ISeedApiDatabaseCommand>();

			var commandLine = Array.Empty<string>();

			var target = new ApplicationLogic(migrateDatabaseCommandMock, seedApiDatabaseCommandMock, Substitute.For<IFileSystemFacade>());

			// Act

			var exitCode = target.Run(commandLine, CancellationToken.None).Result;

			// Assert

			Assert.AreEqual(1, exitCode);
			migrateDatabaseCommandMock.DidNotReceive().Execute(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
			seedApiDatabaseCommandMock.DidNotReceive().Execute(Arg.Any<CancellationToken>());
		}

		[Test]
		public void Run_ForUnknownCommand_ReturnsWithoutExecutingAnyCommand()
		{
			// Arrange

			var migrateDatabaseCommandMock = Substitute.For<IMigrateDatabaseCommand>();
			var seedApiDatabaseCommandMock = Substitute.For<ISeedApiDatabaseCommand>();

			var commandLine = new[] { "--some-unknown-command", "SomeArg1", "SomeArg2", };

			var target = new ApplicationLogic(migrateDatabaseCommandMock, seedApiDatabaseCommandMock, Substitute.For<IFileSystemFacade>());

			// Act

			var exitCode = target.Run(commandLine, CancellationToken.None).Result;

			// Assert

			Assert.AreEqual(1, exitCode);
			migrateDatabaseCommandMock.DidNotReceive().Execute(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
			seedApiDatabaseCommandMock.DidNotReceive().Execute(Arg.Any<CancellationToken>());
		}
	}
}
