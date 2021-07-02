using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.IntegrationTests.Data;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.Services.IntegrationTests
{
	[TestClass]
	public class ArtistsServiceTests : BasicServiceTests<IArtistsService>
	{
		[TestMethod]
		public async Task CreateArtist_ForNonExistingArtistName_CreatesArtistSuccessfully()
		{
			// Arrange

			var newArtist = new ArtistModel
			{
				Name = "Nautilus Pompilius",
			};

			var target = CreateTestTarget();

			// Act

			await target.CreateArtist(newArtist, CancellationToken.None);

			// Assert

			var testData = new TestData(LibraryStorageRoot);
			var expectedArtists = new[]
			{
				testData.Artist1,
				testData.Artist2,
				new ArtistModel
				{
					Id = new ItemId("3"),
					Name = "Nautilus Pompilius",
				},
			};

			var allArtists = await target.GetAllArtists(CancellationToken.None);
			allArtists.Should().BeEquivalentTo(expectedArtists);
		}

		[TestMethod]
		public async Task CreateArtist_ForExistingArtistName_Throws()
		{
			// Arrange

			var newArtist = new ArtistModel
			{
				Name = "Guano Apes",
			};

			var target = CreateTestTarget();

			// Act

			Task Call() => target.CreateArtist(newArtist, CancellationToken.None);

			// Assert

			await Assert.ThrowsExceptionAsync<DbUpdateException>(Call);
		}

		[TestMethod]
		public async Task GetAllArtists_ReturnsArtistsCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var artists = await target.GetAllArtists(CancellationToken.None);

			// Assert

			var testData = new TestData(LibraryStorageRoot);
			var expectedArtists = new[]
			{
				testData.Artist1,
				testData.Artist2,
			};

			artists.Should().BeEquivalentTo(expectedArtists);
		}
	}
}
