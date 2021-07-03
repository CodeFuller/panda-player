using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MusicLibrary.Core.Facades;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Services.Extensions;
using MusicLibrary.Services.IntegrationTests.Data;

namespace MusicLibrary.Services.IntegrationTests
{
	public abstract class BasicServiceTests<TService>
	{
		private const string TestDatabaseFileName = "TestDatabase-dirty.db";

		protected string LibraryStorageRoot { get; private set; }

		private ServiceProvider ServiceProvider { get; set; }

		protected TService CreateTestTarget(Action<IServiceCollection> setupServices = null)
		{
			if (ServiceProvider != null)
			{
				throw new InvalidOperationException("Previous services were not disposed");
			}

			var services = new ServiceCollection()
				.AddMusicLibraryDbContext(@$"Data Source={TestDatabaseFileName};Foreign Keys=True;")
				.AddLocalDbDal(settings =>
				{
					settings.Root = LibraryStorageRoot;
				})
				.AddMusicLibraryServices();

			services.AddLogging();

			setupServices?.Invoke(services);

			ServiceProvider = services.BuildServiceProvider();

			return ServiceProvider.GetRequiredService<TService>();
		}

		protected T GetService<T>()
		{
			return ServiceProvider.GetRequiredService<T>();
		}

		protected Action<IServiceCollection> StubClock(DateTimeOffset now)
		{
			var clock = new Mock<IClock>();
			clock.Setup(x => x.Now).Returns(now);

			return services => services.AddSingleton<IClock>(clock.Object);
		}

		[TestInitialize]
		public void Initialize()
		{
			DeleteStorageData();

			LibraryStorageRoot ??= Path.Combine(Path.GetTempPath(), "MusicLibrary.IT", @"music\");
			CopyStorageData();

			File.Copy("TestDatabase.db", TestDatabaseFileName, overwrite: true);
		}

		[TestCleanup]
		public void Cleanup()
		{
			DisposeServices();

			DeleteStorageData();
			File.Delete(TestDatabaseFileName);
		}

#pragma warning disable CA1024 // Use properties where appropriate
		protected TestData GetTestData()
#pragma warning restore CA1024 // Use properties where appropriate
		{
			return new(LibraryStorageRoot);
		}

		private void CopyStorageData()
		{
			CopyDirectory("Content", LibraryStorageRoot);

			SetFilesAttributes(LibraryStorageRoot, FileAttributes.ReadOnly);
		}

		private static void CopyDirectory(string sourcePath, string targetPath)
		{
			Directory.CreateDirectory(targetPath);

			foreach (var sourceFilePath in Directory.GetFiles(sourcePath))
			{
				var targetFilePath = Path.Combine(targetPath, Path.GetFileName(sourceFilePath));
				File.Copy(sourceFilePath, targetFilePath, overwrite: false);
			}

			foreach (var sourceSubDirectoryPath in Directory.GetDirectories(sourcePath))
			{
				var targetSubDirectoryPath = Path.Combine(targetPath, Path.GetFileName(sourceSubDirectoryPath));
				CopyDirectory(sourceSubDirectoryPath, targetSubDirectoryPath);
			}
		}

		private void DeleteStorageData()
		{
			if (LibraryStorageRoot == null || !Directory.Exists(LibraryStorageRoot))
			{
				return;
			}

			// We cannot call Directory.Delete(FileSystemStorageRoot, true), because the directory contains read-only files.
			SetFilesAttributes(LibraryStorageRoot, FileAttributes.Normal);
			Directory.Delete(LibraryStorageRoot, recursive: true);
		}

		private static void SetFilesAttributes(string directoryPath, FileAttributes attributes)
		{
			foreach (var filePath in Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories))
			{
				File.SetAttributes(filePath, attributes);
			}
		}

		private void DisposeServices()
		{
			if (ServiceProvider == null)
			{
				return;
			}

			ServiceProvider.Dispose();
			ServiceProvider = null;
		}
	}
}
