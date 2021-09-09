using System;
using System.Collections.Generic;
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

			var dirtyReferenceData = GetReferenceData();
			dirtyReferenceData.DiscWithMissingFields.AdviseSet.Should().BeNull();

			var target = CreateTestTarget();

			// Act

			await target.AddDiscs(dirtyReferenceData.AdviseSet2, new[] { dirtyReferenceData.DiscWithMissingFields }, CancellationToken.None);

			// Assert

			dirtyReferenceData.DiscWithMissingFields.AdviseSet.Should().Be(dirtyReferenceData.AdviseSet2);
			dirtyReferenceData.DiscWithMissingFields.AdviseSetOrder.Should().Be(1);

			var referenceData = GetReferenceData();
			referenceData.DiscWithMissingFields.AdviseSet = referenceData.AdviseSet2;
			referenceData.DiscWithMissingFields.AdviseSetOrder = 1;

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

			var dirtyReferenceData = GetReferenceData();
			dirtyReferenceData.DiscWithMissingFields.AdviseSet.Should().BeNull();

			var target = CreateTestTarget();

			await target.AddDiscs(dirtyReferenceData.AdviseSet1, new[] { dirtyReferenceData.DiscWithMissingFields }, CancellationToken.None);

			// Act

			await target.ReorderDiscs(dirtyReferenceData.AdviseSet1, new[] { dirtyReferenceData.DiscWithMissingFields, dirtyReferenceData.NormalDisc }, CancellationToken.None);

			// Assert

			dirtyReferenceData.DiscWithMissingFields.AdviseSet.Should().Be(dirtyReferenceData.AdviseSet1);
			dirtyReferenceData.DiscWithMissingFields.AdviseSetOrder.Should().Be(1);

			dirtyReferenceData.NormalDisc.AdviseSet.Should().Be(dirtyReferenceData.AdviseSet1);
			dirtyReferenceData.NormalDisc.AdviseSetOrder.Should().Be(2);

			var referenceData = GetReferenceData();
			referenceData.DiscWithMissingFields.AdviseSet = referenceData.AdviseSet1;
			referenceData.DiscWithMissingFields.AdviseSetOrder = 1;
			referenceData.NormalDisc.AdviseSetOrder = 2;

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

			var dirtyReferenceData = GetReferenceData();
			dirtyReferenceData.DiscWithMissingFields.AdviseSet.Should().BeNull();

			var removedDisc = dirtyReferenceData.NormalDisc;

			var target = CreateTestTarget();

			// Act

			await target.RemoveDiscs(dirtyReferenceData.AdviseSet1, new[] { removedDisc }, CancellationToken.None);

			// Assert

			removedDisc.AdviseSet.Should().BeNull();
			removedDisc.AdviseSetOrder.Should().BeNull();

			var referenceData = GetReferenceData();
			referenceData.NormalDisc.AdviseSet = null;
			referenceData.NormalDisc.AdviseSetOrder = null;

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

			var dirtyReferenceData = GetReferenceData();
			dirtyReferenceData.DiscWithMissingFields.AdviseSet.Should().BeNull();

			var removedDisc = dirtyReferenceData.NormalDisc;
			var leftDisc = dirtyReferenceData.DiscWithMissingFields;

			var target = CreateTestTarget();

			await target.AddDiscs(dirtyReferenceData.AdviseSet1, new[] { leftDisc }, CancellationToken.None);

			// Act

			await target.RemoveDiscs(dirtyReferenceData.AdviseSet1, new[] { removedDisc }, CancellationToken.None);

			// Assert

			removedDisc.AdviseSet.Should().BeNull();
			removedDisc.AdviseSetOrder.Should().BeNull();

			var referenceData = GetReferenceData();
			referenceData.NormalDisc.AdviseSet = null;
			referenceData.NormalDisc.AdviseSetOrder = null;
			referenceData.DiscWithMissingFields.AdviseSet = referenceData.AdviseSet1;
			referenceData.DiscWithMissingFields.AdviseSetOrder = 1;

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

			var dirtyReferenceData = GetReferenceData();
			var adviseSet = dirtyReferenceData.AdviseSet1;

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

			var dirtyReferenceData = GetReferenceData();
			var adviseSet = dirtyReferenceData.AdviseSet2;

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

			var dirtyReferenceData = GetReferenceData();
			var adviseSet = dirtyReferenceData.AdviseSet1;

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

			referenceData.NormalDisc.AdviseSet = null;
			referenceData.NormalDisc.AdviseSetOrder = null;

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

		private async Task<IReadOnlyCollection<DiscModel>> GetAllDiscs()
		{
			var discService = GetService<IDiscsService>();
			return await discService.GetAllDiscs(CancellationToken.None);
		}
	}
}
