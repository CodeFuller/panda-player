using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Dal;
using MusicLibrary.Dal.Extensions;
using NUnit.Framework;

namespace MusicLibrary.IntegrationTests.MusicLibrary.Dal
{
	[TestFixture]
	public class MusicLibraryRepositoryTests
	{
		[Test]
		public void GetDiscs_LoadsDiscsDataCorrectly()
		{
			// Arrange

			var services = new ServiceCollection();

			var binPath = AppDomain.CurrentDomain.BaseDirectory;
			services.AddDal(settings => settings.DataSource = Path.Combine(binPath, "MusicLibrary.db"));

			var serviceProvider = services.BuildServiceProvider();
			var target = serviceProvider.GetRequiredService<IMusicLibraryRepository>();

			// Act

			var discs = target.GetDiscs().Result.ToList();

			// Assert

			Assert.IsNotEmpty(discs);
			Assert.IsNotEmpty(discs.SelectMany(d => d.Songs));
			Assert.IsNotEmpty(discs.SelectMany(d => d.Songs).Select(s => s.Artist).Where(a => a != null));
			Assert.IsNotEmpty(discs.SelectMany(d => d.Songs).Select(s => s.Genre).Where(g => g != null));
			Assert.IsNotEmpty(discs.Select(d => d.CoverImage).Where(c => c != null));
		}

		// The test does not actually invoke CopyData() method. It just counts the number of tables in the database.
		// If the test starts failing, update the method MusicLibraryRepository.CopyData to cover all tables and populate knownTables collection.
		[Test]
		public void CopyData_CopiesDataFromAllDatabaseTables()
		{
			// Arrange

			var knownTables = new HashSet<string>
			{
				"Artists",
				"DiscImages",
				"Discs",
				"Genres",
				"Playbacks",
				"Songs",
			};

			var binPath = AppDomain.CurrentDomain.BaseDirectory;
			var connectionString = Path.Combine(binPath, "MusicLibrary.db").ToConnectionString();

			using var connection = new SqliteConnection(connectionString);
			connection.Open();

			var command = connection.CreateCommand();
			command.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";

			var actualTables = new List<string>();

			// Act

			var dataReader = command.ExecuteReader();
			while (dataReader.Read())
			{
				actualTables.Add(dataReader.GetString(0));
			}

			// Assert

			var unknownTables = actualTables.Where(t => !knownTables.Contains(t)).ToList();
			CollectionAssert.IsEmpty(unknownTables, $"Following table(s) are not covered by {nameof(MusicLibraryRepository.CopyData)} method: {String.Join(", ", unknownTables)}");
		}
	}
}
