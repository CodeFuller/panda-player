using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.IntegrationTests.Data;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.Services.IntegrationTests
{
	[TestClass]
	public class AdviseGroupServiceTests : BasicServiceTests<IAdviseGroupService>
	{
		[TestMethod]
		public async Task CreateAdviseGroup_ForNonExistingAdviseGroupName_CreatesAdviseGroupSuccessfully()
		{
			// Arrange

			var newAdviseGroup = new AdviseGroupModel
			{
				Name = "Зоопарк",
			};

			var target = CreateTestTarget();

			// Act

			await target.CreateAdviseGroup(newAdviseGroup, CancellationToken.None);

			// Assert

			var expectedAdviseGroup = new AdviseGroupModel
			{
				Id = ReferenceData.NextAdviseGroupId,
				Name = "Зоопарк",
			};

			newAdviseGroup.Should().BeEquivalentTo(expectedAdviseGroup);

			var referenceData = GetReferenceData();
			var expectedAdviseGroups = new[]
			{
				referenceData.AdviseGroup2,
				referenceData.AdviseGroup1,
				expectedAdviseGroup,
			};

			var allAdviseGroups = await target.GetAllAdviseGroups(CancellationToken.None);
			allAdviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task CreateAdviseGroup_ForExistingAdviseGroupName_Throws()
		{
			// Arrange

			var newAdviseGroup = new AdviseGroupModel
			{
				Name = "Late Neuro Dubel",
			};

			var target = CreateTestTarget();

			// Act

			Func<Task> call = () => target.CreateAdviseGroup(newAdviseGroup, CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<DbUpdateException>();

			var referenceData = GetReferenceData();
			var expectedAdviseGroups = new[]
			{
				referenceData.AdviseGroup2,
				referenceData.AdviseGroup1,
			};

			var allAdviseGroups = await target.GetAllAdviseGroups(CancellationToken.None);
			allAdviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task GetAllAdviseGroups_ReturnsAdviseGroupsCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var adviseGroups = await target.GetAllAdviseGroups(CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedAdviseGroups = new[]
			{
				referenceData.AdviseGroup2,
				referenceData.AdviseGroup1,
			};

			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());
		}
	}
}
