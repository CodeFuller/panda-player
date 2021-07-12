using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Services.IntegrationTests.Data;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.Services.IntegrationTests
{
	[TestClass]
	public class SessionDataServiceTests : BasicServiceTests<ISessionDataService>
	{
		[TestMethod]
		public async Task SaveData_ForNonExistingKey_SavesDataCorrectly()
		{
			// Arrange

			var data = new TestSessionData
			{
				NumericProperty = 123456789,
				StringProperty = "StringProperty From Test",
				CollectionProperty = new[]
				{
					"CollectionValue1 From Test",
					"CollectionValue2 From Test",
				},
			};

			var target = CreateTestTarget();

			// Act

			await target.SaveData(ReferenceData.NonExistingSessionDataKey, data, CancellationToken.None);

			// Assert

			var dataFromRepository = await target.GetData<TestSessionData>(ReferenceData.NonExistingSessionDataKey, CancellationToken.None);
			dataFromRepository.Should().BeEquivalentTo(data);

			var referenceData = GetReferenceData();
			var existingData = await target.GetData<TestSessionData>(ReferenceData.ExistingSessionDataKey, CancellationToken.None);
			existingData.Should().BeEquivalentTo(referenceData.ExistingSessionData);
		}

		[TestMethod]
		public async Task SaveData_ForExistingKey_SavesDataCorrectly()
		{
			// Arrange

			var data = new TestSessionData
			{
				NumericProperty = 123456789,
				StringProperty = "StringProperty From Test",
				CollectionProperty = new[]
				{
					"CollectionValue1 From Test",
					"CollectionValue2 From Test",
				},
			};

			var target = CreateTestTarget();

			// Act

			await target.SaveData(ReferenceData.ExistingSessionDataKey, data, CancellationToken.None);

			// Assert

			var loadedData = await target.GetData<TestSessionData>(ReferenceData.ExistingSessionDataKey, CancellationToken.None);
			loadedData.Should().BeEquivalentTo(data);
		}

		[TestMethod]
		public async Task GetData_ForNonExistingKey_ReturnsNull()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var data = await target.GetData<TestSessionData>(ReferenceData.NonExistingSessionDataKey, CancellationToken.None);

			// Assert

			data.Should().BeNull();
		}

		[TestMethod]
		public async Task GetData_ForExistingKey_ReturnsCorrectData()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var data = await target.GetData<TestSessionData>(ReferenceData.ExistingSessionDataKey, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			data.Should().BeEquivalentTo(referenceData.ExistingSessionData);
		}

		[TestMethod]
		public async Task PurgeData_ForNonExistingKey_DoesNothing()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			await target.PurgeData(ReferenceData.NonExistingSessionDataKey, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var existingData = await target.GetData<TestSessionData>(ReferenceData.ExistingSessionDataKey, CancellationToken.None);
			existingData.Should().BeEquivalentTo(referenceData.ExistingSessionData);
		}

		[TestMethod]
		public async Task PurgeData_ForExistingKey_DeletesDataCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			await target.PurgeData(ReferenceData.ExistingSessionDataKey, CancellationToken.None);

			// Assert

			var existingData = await target.GetData<TestSessionData>(ReferenceData.ExistingSessionDataKey, CancellationToken.None);
			existingData.Should().BeNull();
		}
	}
}
