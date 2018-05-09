using System;
using System.IO;
using System.Linq;
using CF.MusicLibrary.Dal;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.IntegrationTests.CF.MusicLibrary.Dal
{
	[TestFixture]
	public class MusicLibraryRepositoryEFTests
	{
		[Test]
		public void GetDiscs_LoadsDiscsDataCorrectly()
		{
			//	Arrange

			var binPath = AppDomain.CurrentDomain.BaseDirectory;
			var settings = new SqLiteConnectionSettings
			{
				DataSource = Path.Combine(binPath, "MusicLibrary.db"),
			};

			var options = Substitute.For<IOptions<SqLiteConnectionSettings>>();
			options.Value.Returns(settings);
			var connectionFactory = new SqLiteConnectionFactory(options);
			var target = new MusicLibraryRepositoryEF(connectionFactory);

			//	Act

			var discs = target.GetDiscs().Result.ToList();

			//	Assert

			Assert.IsNotEmpty(discs);
			Assert.IsNotEmpty(discs.SelectMany(d => d.Songs));
			Assert.IsNotEmpty(discs.SelectMany(d => d.Songs).Select(s => s.Artist).Where(a => a != null));
			Assert.IsNotEmpty(discs.SelectMany(d => d.Songs).Select(s => s.Genre).Where(g => g != null));
		}
	}
}
