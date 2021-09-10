using System;
using System.Linq;
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
	public class AdviseSetServiceTests : BasicServiceTests<IAdviseSetService>
	{
		[TestMethod]
		public async Task CreateAdviseSet_ForNonExistingAdviseSetName_CreatesAdviseSetSuccessfully()
		{
			// Arrange

			var newAdviseSet = new AdviseSetModel
			{
				Name = "Вершки и корешки",
			};

			var target = CreateTestTarget();

			// Act

			await target.CreateAdviseSet(newAdviseSet, CancellationToken.None);

			// Assert

			var expectedAdviseSet = new AdviseSetModel
			{
				Id = ReferenceData.NextAdviseSetId,
				Name = "Вершки и корешки",
			};

			newAdviseSet.Should().BeEquivalentTo(expectedAdviseSet);

			var referenceData = GetReferenceData();
			var expectedAdviseSets = new[]
			{
				referenceData.AdviseSet2,
				referenceData.AdviseSet1,
				expectedAdviseSet,
			};

			var allAdviseSets = await target.GetAllAdviseSets(CancellationToken.None);
			allAdviseSets.Should().BeEquivalentTo(expectedAdviseSets, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task CreateAdviseSet_ForExistingAdviseSetName_Throws()
		{
			// Arrange

			var referenceData = GetReferenceData();

			var newAdviseSet = new AdviseSetModel
			{
				Name = referenceData.AdviseSet1.Name,
			};

			var target = CreateTestTarget();

			// Act

			Func<Task> call = () => target.CreateAdviseSet(newAdviseSet, CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<DbUpdateException>();

			var expectedAdviseSets = new[]
			{
				referenceData.AdviseSet2,
				referenceData.AdviseSet1,
			};

			var allAdviseSets = await target.GetAllAdviseSets(CancellationToken.None);
			allAdviseSets.Should().BeEquivalentTo(expectedAdviseSets, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task GetAllAdviseSets_ReturnsAdviseSetsCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var adviseSets = await target.GetAllAdviseSets(CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedAdviseSets = new[]
			{
				referenceData.AdviseSet2,
				referenceData.AdviseSet1,
			};

			adviseSets.Should().BeEquivalentTo(expectedAdviseSets, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task AddDiscs_ForEmptyAdviseSet_AddsDiscsCorrectly()
		{
			// Arrange

			var discForAdding = await GetDisc(ReferenceData.DiscWithMissingFieldsId);
			discForAdding.AdviseSetInfo.Should().BeNull();

			var adviseSet = await GetAdviseSet(ReferenceData.AdviseSet2Id);

			var target = CreateTestTarget();

			// Act

			await target.AddDiscs(adviseSet, new[] { discForAdding }, CancellationToken.None);

			// Assert

			discForAdding.AdviseSetInfo.Should().BeEquivalentTo(new AdviseSetInfo(adviseSet, 1));

			var referenceData = GetReferenceData();
			referenceData.DiscWithMissingFields.AdviseSetInfo = new AdviseSetInfo(referenceData.AdviseSet2, 1);

			var expectedDiscs = new[]
			{
				referenceData.NormalDisc,
				referenceData.DiscWithMissingFields,
				referenceData.DeletedDisc,
			};

			var discsFromRepository = await GetAllDiscs();
			discsFromRepository.Should().BeEquivalentTo(expectedDiscs, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task AddDiscs_ForNonEmptyAdviseSet_AddsDiscsCorrectly()
		{
			// Arrange

			var discForAdding = await GetDisc(ReferenceData.DiscWithMissingFieldsId);
			discForAdding.AdviseSetInfo.Should().BeNull();

			var adviseSet = await GetAdviseSet(ReferenceData.AdviseSet1Id);

			var target = CreateTestTarget();

			// Act

			await target.AddDiscs(adviseSet, new[] { discForAdding }, CancellationToken.None);

			// Assert

			discForAdding.AdviseSetInfo.Should().BeEquivalentTo(new AdviseSetInfo(adviseSet, 2));

			var referenceData = GetReferenceData();
			referenceData.DiscWithMissingFields.AdviseSetInfo = new AdviseSetInfo(referenceData.AdviseSet1, 2);

			var expectedDiscs = new[]
			{
				referenceData.NormalDisc,
				referenceData.DiscWithMissingFields,
				referenceData.DeletedDisc,
			};

			var discsFromRepository = await GetAllDiscs();
			discsFromRepository.Should().BeEquivalentTo(expectedDiscs, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task ReorderDiscs_ReordersDiscsCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			var disc1 = await GetDisc(ReferenceData.NormalDiscId);
			var disc2 = await GetDisc(ReferenceData.DiscWithMissingFieldsId);

			// Adding one more disc to advise set.
			var adviseSet = await GetAdviseSet(ReferenceData.AdviseSet1Id);
			await target.AddDiscs(adviseSet, new[] { disc2 }, CancellationToken.None);

			// Act

			await target.ReorderDiscs(adviseSet, new[] { disc2, disc1 }, CancellationToken.None);

			// Assert

			disc1.AdviseSetInfo.Should().BeEquivalentTo(new AdviseSetInfo(adviseSet, 2));
			disc2.AdviseSetInfo.Should().BeEquivalentTo(new AdviseSetInfo(adviseSet, 1));

			var referenceData = GetReferenceData();
			referenceData.DiscWithMissingFields.AdviseSetInfo = new AdviseSetInfo(referenceData.AdviseSet1, 1);
			referenceData.NormalDisc.AdviseSetInfo = referenceData.NormalDisc.AdviseSetInfo.WithOrder(2);

			var expectedDiscs = new[]
			{
				referenceData.NormalDisc,
				referenceData.DiscWithMissingFields,
				referenceData.DeletedDisc,
			};

			var discsFromRepository = await GetAllDiscs();
			discsFromRepository.Should().BeEquivalentTo(expectedDiscs, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task RemoveDiscs_IfLastAdviseSetDiscIsRemoved_RemovesDiscCorrectly()
		{
			// Arrange

			var removedDisc = await GetDisc(ReferenceData.NormalDiscId);
			var adviseSet = await GetAdviseSet(ReferenceData.AdviseSet1Id);

			var target = CreateTestTarget();

			// Act

			await target.RemoveDiscs(adviseSet, new[] { removedDisc }, CancellationToken.None);

			// Assert

			removedDisc.AdviseSetInfo.Should().BeNull();

			var referenceData = GetReferenceData();
			referenceData.NormalDisc.AdviseSetInfo = null;

			var expectedDiscs = new[]
			{
				referenceData.NormalDisc,
				referenceData.DiscWithMissingFields,
				referenceData.DeletedDisc,
			};

			var discsFromRepository = await GetAllDiscs();
			discsFromRepository.Should().BeEquivalentTo(expectedDiscs, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task RemoveDiscs_IfNotLastAdviseSetDiscIsRemoved_RemovesDiscCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			var removedDisc = await GetDisc(ReferenceData.NormalDiscId);
			var leftDisc = await GetDisc(ReferenceData.DiscWithMissingFieldsId);

			// Adding one more disc to advise set.
			var adviseSet = await GetAdviseSet(ReferenceData.AdviseSet1Id);
			await target.AddDiscs(adviseSet, new[] { leftDisc }, CancellationToken.None);

			// Act

			await target.RemoveDiscs(adviseSet, new[] { removedDisc }, CancellationToken.None);

			// Assert

			removedDisc.AdviseSetInfo.Should().BeNull();

			var referenceData = GetReferenceData();
			referenceData.NormalDisc.AdviseSetInfo = null;
			referenceData.DiscWithMissingFields.AdviseSetInfo = new AdviseSetInfo(referenceData.AdviseSet1, 1);

			var expectedDiscs = new[]
			{
				referenceData.NormalDisc,
				referenceData.DiscWithMissingFields,
				referenceData.DeletedDisc,
			};

			var discsFromRepository = await GetAllDiscs();
			discsFromRepository.Should().BeEquivalentTo(expectedDiscs, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task UpdateAdviseSet_IfAdviseSetIsRenamed_RenamesAdviseSetCorrectly()
		{
			// Arrange

			var adviseSet = await GetAdviseSet(ReferenceData.AdviseSet1Id);

			var target = CreateTestTarget();

			// Act

			adviseSet.Name = "New Advise Set Name";

			await target.UpdateAdviseSet(adviseSet, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			referenceData.AdviseSet1.Name = "New Advise Set Name";

			var expectedAdviseSets = new[]
			{
				referenceData.AdviseSet2,
				referenceData.AdviseSet1,
			};

			var adviseSetsFromRepository = await target.GetAllAdviseSets(CancellationToken.None);
			adviseSetsFromRepository.Should().BeEquivalentTo(expectedAdviseSets, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task DeleteAdviseSet_ForEmptyAdviseSet_DeletesAdviseSetCorrectly()
		{
			// Arrange

			var adviseSet = await GetAdviseSet(ReferenceData.AdviseSet2Id);

			var target = CreateTestTarget();

			// Act

			await target.DeleteAdviseSet(adviseSet, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedAdviseSets = new[]
			{
				referenceData.AdviseSet1,
			};

			var adviseSetsFromRepository = await target.GetAllAdviseSets(CancellationToken.None);
			adviseSetsFromRepository.Should().BeEquivalentTo(expectedAdviseSets, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task DeleteAdviseSet_ForNonEmptyAdviseSet_DeletesAdviseSetCorrectly()
		{
			// Arrange

			var adviseSet = await GetAdviseSet(ReferenceData.AdviseSet1Id);

			var target = CreateTestTarget();

			// Act

			await target.DeleteAdviseSet(adviseSet, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedAdviseSets = new[]
			{
				referenceData.AdviseSet2,
			};

			var adviseSetsFromRepository = await target.GetAllAdviseSets(CancellationToken.None);
			adviseSetsFromRepository.Should().BeEquivalentTo(expectedAdviseSets, x => x.WithStrictOrdering());

			referenceData.NormalDisc.AdviseSetInfo = null;

			var expectedDiscs = new[]
			{
				referenceData.NormalDisc,
				referenceData.DiscWithMissingFields,
				referenceData.DeletedDisc,
			};

			var discsFromRepository = await GetAllDiscs();
			discsFromRepository.Should().BeEquivalentTo(expectedDiscs, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			await CheckLibraryConsistency();
		}

		private async Task<AdviseSetModel> GetAdviseSet(ItemId adviseSetId)
		{
			var adviseSetService = GetService<IAdviseSetService>();
			var allAdviseSets = await adviseSetService.GetAllAdviseSets(CancellationToken.None);

			return allAdviseSets.Single(x => x.Id == adviseSetId);
		}
	}
}
