using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using CF.MusicLibrary.Dal;
using CF.MusicLibrary.Tests;
using NUnit.Framework;

namespace CF.MusicLibrary.IntegrationTests.CF.MusicLibrary.Dal
{
	[TestFixture]
	public class MusicLibraryRepositoryEFTests
	{
		[Test]
		public void GetDiscs_LoadsDiscsDataCorrectly()
		{
			// Arrange

			var binPath = AppDomain.CurrentDomain.BaseDirectory;
			var settings = new SqLiteConnectionSettings
			{
				DataSource = Path.Combine(binPath, "MusicLibrary.db"),
			};

			var connectionFactory = new SqLiteConnectionFactory(settings.StubOptions());
			var target = new MusicLibraryRepositoryEF(connectionFactory);

			// Act

			var discs = target.GetDiscs().Result.ToList();

			// Assert

			Assert.IsNotEmpty(discs);
			Assert.IsNotEmpty(discs.SelectMany(d => d.Songs));
			Assert.IsNotEmpty(discs.SelectMany(d => d.Songs).Select(s => s.Artist).Where(a => a != null));
			Assert.IsNotEmpty(discs.SelectMany(d => d.Songs).Select(s => s.Genre).Where(g => g != null));
			Assert.IsNotEmpty(discs.Select(d => d.CoverImage).Where(c => c != null));
		}

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
			var settings = new SqLiteConnectionSettings
			{
				DataSource = Path.Combine(binPath, "MusicLibrary.db"),
			};

			var connectionFactory = new SqLiteConnectionFactory(settings.StubOptions());

			// Act

			List<string> actualTables;
			using (var connection = connectionFactory.CreateConnection())
			{
				connection.Open();
				var schema = connection.GetSchema("Tables");
				actualTables = schema.Rows.Cast<DataRow>().Select(r => (string)r["TABLE_NAME"]).ToList();
			}

			// Assert

			var unknownTables = actualTables.Where(t => !knownTables.Contains(t)).ToList();
			CollectionAssert.IsEmpty(unknownTables, $"Following table(s) are not covered by {nameof(MusicLibraryRepositoryEF.CopyData)} method: {String.Join(", ", unknownTables)}");
		}
	}
}
