using System;
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

			var expectedArtist = new ArtistModel
			{
				Id = ReferenceData.NextArtistId,
				Name = "Nautilus Pompilius",
			};

			newArtist.Should().BeEquivalentTo(expectedArtist);

			var referenceData = GetReferenceData();
			var expectedArtists = new[]
			{
				referenceData.Artist1,
				referenceData.Artist2,
				expectedArtist,
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

			Func<Task> call = () => target.CreateArtist(newArtist, CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<DbUpdateException>();

			var referenceData = GetReferenceData();
			var expectedArtists = new[]
			{
				referenceData.Artist1,
				referenceData.Artist2,
			};

			var allArtists = await target.GetAllArtists(CancellationToken.None);
			allArtists.Should().BeEquivalentTo(expectedArtists);
		}

		[TestMethod]
		public async Task GetAllArtists_ReturnsArtistsCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var artists = await target.GetAllArtists(CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedArtists = new[]
			{
				referenceData.Artist1,
				referenceData.Artist2,
			};

			artists.Should().BeEquivalentTo(expectedArtists);
		}
	}
}
