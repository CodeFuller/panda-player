using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.Services.IntegrationTests
{
	[TestClass]
	public class GenresServiceTests : BasicServiceTests<IGenresService>
	{
		[TestMethod]
		public async Task GetAllGenres_ReturnsGenresCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var genres = await target.GetAllGenres(CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedGenres = new[]
			{
				referenceData.Genre2,
				referenceData.Genre3,
				referenceData.Genre1,
			};

			genres.Should().BeEquivalentTo(expectedGenres, x => x.WithStrictOrdering());
		}
	}
}
