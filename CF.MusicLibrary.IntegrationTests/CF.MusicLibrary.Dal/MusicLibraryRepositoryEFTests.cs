using System.Linq;
using CF.MusicLibrary.Dal;
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

			var target = new MusicLibraryRepositoryEF();

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
