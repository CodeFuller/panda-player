using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Extensions;
using PandaPlayer.Services.Diagnostic;
using PandaPlayer.Services.Diagnostic.Inconsistencies;
using PandaPlayer.Services.Extensions;
using PandaPlayer.Services.IntegrationTests.Data;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Internal;

namespace PandaPlayer.Services.IntegrationTests
{
	public abstract class BasicServiceTests<TService>
	{
		private const string TestDatabaseFileName = "TestDatabase-dirty.db";

		protected string LibraryStorageRoot { get; private set; }

		private ServiceProvider ServiceProvider { get; set; }

		protected TService CreateTestTarget(Action<IServiceCollection> setupServices = null)
		{
			if (setupServices != null && ServiceProvider != null)
			{
				throw new InvalidOperationException("Cannot apply custom services configuration for existing ServiceProvider");
			}

			ServiceProvider ??= InitializeServiceProvider(setupServices);

			return ServiceProvider.GetRequiredService<TService>();
		}

		protected T GetService<T>()
		{
			ServiceProvider ??= InitializeServiceProvider();

			return ServiceProvider.GetRequiredService<T>();
		}

		private ServiceProvider InitializeServiceProvider(Action<IServiceCollection> setupServices = null)
		{
			var services = new ServiceCollection()
				.AddMusicDbContext(@$"Data Source={TestDatabaseFileName};Foreign Keys=True;")
				.AddLocalDbDal(settings =>
				{
					settings.Root = LibraryStorageRoot;
				})
				.AddPandaPlayerServices()
				.AddDiscTitleToAlbumMapper(settings =>
				{
					settings.AlbumTitlePatterns = new[] { @"^(.+) \(CD ?\d+\)$" };
					settings.EmptyAlbumTitlePatterns = new[] { "Disc With Missing Fields" };
				})
				.AddLogging();

			setupServices?.Invoke(services);

			var serviceProvider = services.BuildServiceProvider();

			InvokeDiscLibraryInitializer(serviceProvider).Wait();

			return serviceProvider;
		}

		private static async Task InvokeDiscLibraryInitializer(IServiceProvider serviceProvider)
		{
			var discLibraryInitializer = serviceProvider.GetRequiredService<IEnumerable<IApplicationInitializer>>()
				.OfType<DiscLibraryInitializer>()
				.Single();

			await discLibraryInitializer.Initialize(CancellationToken.None);
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

			LibraryStorageRoot ??= Path.Combine(Path.GetTempPath(), "PandaPlayer.IT", @"music\");
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

		protected ReferenceData GetReferenceData(bool fillSongPlaybacks = false)
		{
			return new(LibraryStorageRoot, fillSongPlaybacks);
		}

		private void CopyStorageData()
		{
			CopyDirectory("Content", LibraryStorageRoot);

			// There is no trivial way to provide empty directory with Git.
			var directoryPath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "Empty Folder");
			Directory.CreateDirectory(directoryPath);

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

		protected async Task CheckLibraryConsistency(params Type[] allowedInconsistencies)
		{
			await CheckLibraryInconsistencies(allowedInconsistencies);

			await CheckInMemoryDiscModel();
		}

		private async Task CheckLibraryInconsistencies(params Type[] allowedInconsistencies)
		{
			var inconsistencies = new List<LibraryInconsistency>();
			void InconsistenciesHandler(LibraryInconsistency inconsistency) => inconsistencies.Add(inconsistency);

			var diagnosticService = GetService<IDiagnosticService>();

			await diagnosticService.CheckLibrary(LibraryCheckFlags.All, Mock.Of<IOperationProgress>(), InconsistenciesHandler, CancellationToken.None);

			var unexpectedInconsistencies = inconsistencies.Where(x => !allowedInconsistencies.Contains(x.GetType()));
			unexpectedInconsistencies.Should().BeEmpty();
		}

		private async Task CheckInMemoryDiscModel()
		{
			var inMemoryLibrary = DiscLibraryHolder.DiscLibrary;

			await InvokeDiscLibraryInitializer(ServiceProvider);

			var databaseLibrary = DiscLibraryHolder.DiscLibrary;

			inMemoryLibrary.Should().NotBeSameAs(databaseLibrary);
			inMemoryLibrary.Should().BeEquivalentTo(databaseLibrary, x => x.WithStrictOrdering().IgnoringCyclicReferences());
		}

		private void DisposeServices()
		{
			if (ServiceProvider == null)
			{
				return;
			}

			ServiceProvider.Dispose();
			ServiceProvider = null;

			// Clearing all connection pools, so that DB file is released and can be removed.
			// https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-6.0/breaking-changes#sqlite-connections-are-pooled
			SqliteConnection.ClearAllPools();
		}

		protected async Task<IReadOnlyCollection<FolderModel>> GetAllFolders()
		{
			var folderService = GetService<IFoldersService>();
			var rootFolder = await folderService.GetRootFolder(CancellationToken.None);

			static void GetFolders(FolderModel folder, ICollection<FolderModel> folders)
			{
				folders.Add(folder);
				foreach (var subfolder in folder.Subfolders)
				{
					GetFolders(subfolder, folders);
				}
			}

			var allFolders = new List<FolderModel>();
			GetFolders(rootFolder, allFolders);

			return allFolders.OrderBy(x => x.Id.Value).ToList();
		}

		protected async Task<FolderModel> GetFolder(ItemId folderId)
		{
			var allFolders = await GetAllFolders();
			return allFolders.Single(x => x.Id == folderId);
		}

		protected async Task<IReadOnlyCollection<DiscModel>> GetAllDiscs()
		{
			var discService = GetService<IDiscsService>();
			return await discService.GetAllDiscs(CancellationToken.None);
		}

		protected async Task<DiscModel> GetDisc(ItemId discId)
		{
			var allDiscs = await GetAllDiscs();
			return allDiscs.Single(x => x.Id == discId);
		}

		protected Task<IReadOnlyCollection<AdviseGroupModel>> GetAllAdviseGroups()
		{
			var adviseGroupService = GetService<IAdviseGroupService>();
			return adviseGroupService.GetAllAdviseGroups(CancellationToken.None);
		}

		protected async Task<AdviseGroupModel> GetAdviseGroup(ItemId adviseGroupId)
		{
			var allAdviseGroups = await GetAllAdviseGroups();
			return allAdviseGroups.Single(x => x.Id == adviseGroupId);
		}

		protected async Task AssignAdviseGroupToFolder(ItemId folderId, ItemId adviseGroupId)
		{
			var folder = await GetFolder(folderId);
			var adviseGroup = await GetAdviseGroup(adviseGroupId);

			var adviseGroupService = GetService<IAdviseGroupService>();
			await adviseGroupService.AssignAdviseGroup(folder, adviseGroup, CancellationToken.None);
		}
	}
}
