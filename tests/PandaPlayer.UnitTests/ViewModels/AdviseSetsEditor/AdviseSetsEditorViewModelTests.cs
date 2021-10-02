using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeFuller.Library.Wpf;
using CodeFuller.Library.Wpf.Interfaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.AdviseSetsEditor;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.Player;

namespace PandaPlayer.UnitTests.ViewModels.AdviseSetsEditor
{
	[TestClass]
	public class AdviseSetsEditorViewModelTests
	{
		[TestMethod]
		public async Task SelectedAdviseSetSetter_IfNoAdviseSetSelected_ClearsCurrentAdviseSetDiscs()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };

			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) },
				new DiscModel { Id = new ItemId("2"), AllSongs = new[] { new SongModel() } },
			};

			var availableDiscsViewModelMock = new Mock<IAvailableDiscsViewModel>();

			var mocker = StubServices(new[] { adviseSet }, discs);
			mocker.Use(availableDiscsViewModelMock);

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;
			target.CurrentAdviseSetDiscs.Should().NotBeEmpty();

			availableDiscsViewModelMock.Invocations.Clear();

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			target.SelectedAdviseSet = null;

			// Assert

			target.CurrentAdviseSetDiscs.Should().BeEmpty();

			availableDiscsViewModelMock.Verify(x => x.LoadAvailableDiscsForAdviseSet(new ObservableCollection<DiscModel>()), Times.Once);

			var expectedProperties = new[]
			{
				nameof(AdviseSetsEditorViewModel.SelectedAdviseSet),
				nameof(AdviseSetsEditorViewModel.CanDeleteAdviseSet),
				nameof(AdviseSetsEditorViewModel.CanAddDiscs),
				nameof(AdviseSetsEditorViewModel.CanRemoveDisc),
				nameof(AdviseSetsEditorViewModel.CanMoveDiscUp),
				nameof(AdviseSetsEditorViewModel.CanMoveDiscDown),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}

		[TestMethod]
		public async Task SelectedAdviseSetSetter_IfSomeAdviseSetSelected_FillsCurrentAdviseSetDiscsCorrectly()
		{
			// Arrange

			var adviseSet1 = new AdviseSetModel { Id = new ItemId("1") };
			var adviseSet2 = new AdviseSetModel { Id = new ItemId("2") };

			var adviseSetServiceStub = new Mock<IAdviseSetService>();
			adviseSetServiceStub.Setup(x => x.GetAllAdviseSets(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { adviseSet1, adviseSet2 });

			var disc1 = new DiscModel { Id = new ItemId("2_1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet2, 1) };
			var disc2 = new DiscModel { Id = new ItemId("2_2"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet2, 2) };

			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1_1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet1, 1) },
				disc1,
				disc2,
				new DiscModel { Id = new ItemId("3_1"), AllSongs = new[] { new SongModel() } },
			};

			var availableDiscsViewModelMock = new Mock<IAvailableDiscsViewModel>();

			var mocker = StubServices(new[] { adviseSet1, adviseSet2 }, discs);
			mocker.Use(availableDiscsViewModelMock);

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet1;
			target.CurrentAdviseSetDiscs.Should().NotBeEmpty();

			availableDiscsViewModelMock.Invocations.Clear();

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			target.SelectedAdviseSet = adviseSet2;

			// Assert

			target.CurrentAdviseSetDiscs.Should().BeEquivalentTo(new[] { disc1, disc2 }, x => x.WithStrictOrdering());

			Func<ObservableCollection<DiscModel>, bool> verifyDiscs = ds => ds.SequenceEqual(new[] { disc1, disc2 });
			availableDiscsViewModelMock.Verify(x => x.LoadAvailableDiscsForAdviseSet(It.Is<ObservableCollection<DiscModel>>(ds => verifyDiscs(ds))), Times.Once);

			var expectedProperties = new[]
			{
				nameof(AdviseSetsEditorViewModel.SelectedAdviseSet),
				nameof(AdviseSetsEditorViewModel.CanDeleteAdviseSet),
				nameof(AdviseSetsEditorViewModel.CanAddDiscs),
				nameof(AdviseSetsEditorViewModel.CanRemoveDisc),
				nameof(AdviseSetsEditorViewModel.CanMoveDiscUp),
				nameof(AdviseSetsEditorViewModel.CanMoveDiscDown),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}

		[TestMethod]
		public void CanCreateAdviseSetGetter_IfNoAvailableDiscsSelected_ReturnsTrue()
		{
			// Arrange

			var availableDiscsViewModelStub = new Mock<IAvailableDiscsViewModel>();
			availableDiscsViewModelStub.Setup(x => x.SelectedDiscs).Returns(Enumerable.Empty<DiscModel>());
			availableDiscsViewModelStub.Setup(x => x.SelectedDiscsCanBeAddedToAdviseSet(It.IsAny<IReadOnlyCollection<DiscModel>>())).Returns(false);

			var mocker = new AutoMocker();
			mocker.Use(availableDiscsViewModelStub);

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			// Act

			var result = target.CanCreateAdviseSet;

			// Assert

			result.Should().BeTrue();
		}

		[TestMethod]
		public void CanCreateAdviseSetGetter_IfThereAreSelectedAvailableDiscsWhichCanBeAddedToAdviseSet_ReturnsTrue()
		{
			// Arrange

			var selectedDiscs = new[]
			{
				new DiscModel(),
				new DiscModel(),
			};

			var availableDiscsViewModelStub = new Mock<IAvailableDiscsViewModel>();
			availableDiscsViewModelStub.Setup(x => x.SelectedDiscs).Returns(selectedDiscs);
			availableDiscsViewModelStub.Setup(x => x.SelectedDiscsCanBeAddedToAdviseSet(Array.Empty<DiscModel>())).Returns(true);

			var mocker = new AutoMocker();
			mocker.Use(availableDiscsViewModelStub);

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			// Act

			var result = target.CanCreateAdviseSet;

			// Assert

			result.Should().BeTrue();
		}

		[TestMethod]
		public void CanCreateAdviseSetGetter_IfThereAreSelectedAvailableDiscsWhichCanNotBeAddedToAdviseSet_ReturnsFalse()
		{
			// Arrange

			var selectedDiscs = new[]
			{
				new DiscModel(),
				new DiscModel(),
			};

			var availableDiscsViewModelStub = new Mock<IAvailableDiscsViewModel>();
			availableDiscsViewModelStub.Setup(x => x.SelectedDiscs).Returns(selectedDiscs);
			availableDiscsViewModelStub.Setup(x => x.SelectedDiscsCanBeAddedToAdviseSet(Array.Empty<DiscModel>())).Returns(false);

			var mocker = new AutoMocker();
			mocker.Use(availableDiscsViewModelStub);

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			// Act

			var result = target.CanCreateAdviseSet;

			// Assert

			result.Should().BeFalse();
		}

		[TestMethod]
		public async Task CanDeleteAdviseSetGetter_IfNoAdviseSetSelected_ReturnsFalse()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[] { new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) }, };

			var mocker = StubServices(new[] { adviseSet }, discs);
			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = null;

			// Act

			var result = target.CanDeleteAdviseSet;

			// Assert

			result.Should().BeFalse();
		}

		[TestMethod]
		public async Task CanDeleteAdviseSetGetter_IfSomeAdviseSetIsSelected_ReturnsTrue()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[] { new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) }, };

			var mocker = StubServices(new[] { adviseSet }, discs);
			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;

			// Act

			var result = target.CanDeleteAdviseSet;

			// Assert

			result.Should().BeTrue();
		}

		[TestMethod]
		public async Task SelectedAdviseSetDiscSetter_RaisesPropertyChangedEventsCorrectly()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[] { new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) }, };

			var mocker = StubServices(new[] { adviseSet }, discs);
			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			target.SelectedAdviseSetDisc = discs.First();

			// Assert

			var expectedProperties = new[]
			{
				nameof(AdviseSetsEditorViewModel.SelectedAdviseSetDisc),
				nameof(AdviseSetsEditorViewModel.CanDeleteAdviseSet),
				nameof(AdviseSetsEditorViewModel.CanAddDiscs),
				nameof(AdviseSetsEditorViewModel.CanRemoveDisc),
				nameof(AdviseSetsEditorViewModel.CanMoveDiscUp),
				nameof(AdviseSetsEditorViewModel.CanMoveDiscDown),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}

		[TestMethod]
		public async Task CanAddDiscsGetter_IfNoAdviseSetSelected_ReturnsFalse()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[] { new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) }, };

			var availableDiscsViewModelStub = new Mock<IAvailableDiscsViewModel>();
			availableDiscsViewModelStub.Setup(x => x.SelectedDiscsCanBeAddedToAdviseSet(It.IsAny<IReadOnlyCollection<DiscModel>>())).Returns(true);

			var mocker = StubServices(new[] { adviseSet }, discs);
			mocker.Use(availableDiscsViewModelStub);

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = null;

			// Act

			var result = target.CanAddDiscs;

			// Assert

			result.Should().BeFalse();
		}

		[TestMethod]
		public async Task CanAddDiscsGetter_IfSomeAdviseSetIsSelectedButSelectedDiscsCanNotBeAddedToAdviseSet_ReturnsFalse()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[] { new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) }, };

			var availableDiscsViewModelStub = new Mock<IAvailableDiscsViewModel>();
			availableDiscsViewModelStub.Setup(x => x.SelectedDiscsCanBeAddedToAdviseSet(discs)).Returns(false);

			var mocker = StubServices(new[] { adviseSet }, discs);
			mocker.Use(availableDiscsViewModelStub);

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;

			// Act

			var result = target.CanAddDiscs;

			// Assert

			result.Should().BeFalse();
		}

		[TestMethod]
		public async Task CanAddDiscsGetter_IfSomeAdviseSetIsSelectedAndSelectedDiscsCanBeAddedToAdviseSet_ReturnsTrue()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[] { new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) }, };

			var availableDiscsViewModelStub = new Mock<IAvailableDiscsViewModel>();
			availableDiscsViewModelStub.Setup(x => x.SelectedDiscsCanBeAddedToAdviseSet(discs)).Returns(true);

			var mocker = StubServices(new[] { adviseSet }, discs);
			mocker.Use(availableDiscsViewModelStub);

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;

			// Act

			var result = target.CanAddDiscs;

			// Assert

			result.Should().BeTrue();
		}

		[TestMethod]
		public async Task CanRemoveDiscGetter_IfNoAdviseSetSelected_ReturnsFalse()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[] { new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) }, };

			var mocker = StubServices(new[] { adviseSet }, discs);
			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = null;
			target.SelectedAdviseSetDisc = discs.First();

			// Act

			var result = target.CanRemoveDisc;

			// Assert

			result.Should().BeFalse();
		}

		[TestMethod]
		public async Task CanRemoveDiscGetter_IfSomeAdviseSetIsSelectedButNoDiscSelected_ReturnsFalse()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[] { new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) }, };

			var mocker = StubServices(new[] { adviseSet }, discs);
			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;
			target.SelectedAdviseSetDisc = null;

			// Act

			var result = target.CanRemoveDisc;

			// Assert

			result.Should().BeFalse();
		}

		[TestMethod]
		public async Task CanRemoveDiscGetter_IfSomeAdviseSetIsSelectedButNoDiscSelected_ReturnsTrue()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[] { new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) }, };

			var mocker = StubServices(new[] { adviseSet }, discs);
			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;
			target.SelectedAdviseSetDisc = discs.First();

			// Act

			var result = target.CanRemoveDisc;

			// Assert

			result.Should().BeTrue();
		}

		[TestMethod]
		public async Task CanMoveDiscUpGetter_IfNoAdviseSetSelected_ReturnsFalse()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) },
				new DiscModel { Id = new ItemId("2"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 2) },
				new DiscModel { Id = new ItemId("3"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 3) },
			};

			var mocker = StubServices(new[] { adviseSet }, discs);
			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = null;
			target.SelectedAdviseSetDisc = discs[1];

			// Act

			var result = target.CanMoveDiscUp;

			// Assert

			result.Should().BeFalse();
		}

		[TestMethod]
		public async Task CanMoveDiscUpGetter_IfSomeAdviseSetIsSelectedButNoDiscSelected_ReturnsFalse()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) },
				new DiscModel { Id = new ItemId("2"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 2) },
				new DiscModel { Id = new ItemId("3"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 3) },
			};

			var mocker = StubServices(new[] { adviseSet }, discs);
			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;
			target.SelectedAdviseSetDisc = null;

			// Act

			var result = target.CanMoveDiscUp;

			// Assert

			result.Should().BeFalse();
		}

		[TestMethod]
		public async Task CanMoveDiscUpGetter_IfFirstDiscIsSelected_ReturnsFalse()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) },
				new DiscModel { Id = new ItemId("2"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 2) },
				new DiscModel { Id = new ItemId("3"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 3) },
			};

			var mocker = StubServices(new[] { adviseSet }, discs);
			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;
			target.SelectedAdviseSetDisc = discs.First();

			// Act

			var result = target.CanMoveDiscUp;

			// Assert

			result.Should().BeFalse();
		}

		[TestMethod]
		public async Task CanMoveDiscUpGetter_IfNotFirstDiscIsSelected_ReturnsTrue()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) },
				new DiscModel { Id = new ItemId("2"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 2) },
				new DiscModel { Id = new ItemId("3"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 3) },
			};

			var mocker = StubServices(new[] { adviseSet }, discs);
			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;
			target.SelectedAdviseSetDisc = discs[1];

			// Act

			var result = target.CanMoveDiscUp;

			// Assert

			result.Should().BeTrue();
		}

		[TestMethod]
		public async Task CanMoveDiscDownGetter_IfNoAdviseSetSelected_ReturnsFalse()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) },
				new DiscModel { Id = new ItemId("2"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 2) },
				new DiscModel { Id = new ItemId("3"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 3) },
			};

			var mocker = StubServices(new[] { adviseSet }, discs);
			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = null;
			target.SelectedAdviseSetDisc = discs[1];

			// Act

			var result = target.CanMoveDiscDown;

			// Assert

			result.Should().BeFalse();
		}

		[TestMethod]
		public async Task CanMoveDiscDownGetter_IfSomeAdviseSetIsSelectedButNoDiscSelected_ReturnsFalse()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) },
				new DiscModel { Id = new ItemId("2"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 2) },
				new DiscModel { Id = new ItemId("3"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 3) },
			};

			var mocker = StubServices(new[] { adviseSet }, discs);
			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;
			target.SelectedAdviseSetDisc = null;

			// Act

			var result = target.CanMoveDiscDown;

			// Assert

			result.Should().BeFalse();
		}

		[TestMethod]
		public async Task CanMoveDiscDownGetter_IfLastDiscIsSelected_ReturnsFalse()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) },
				new DiscModel { Id = new ItemId("2"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 2) },
				new DiscModel { Id = new ItemId("3"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 3) },
			};

			var mocker = StubServices(new[] { adviseSet }, discs);
			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;
			target.SelectedAdviseSetDisc = discs.Last();

			// Act

			var result = target.CanMoveDiscDown;

			// Assert

			result.Should().BeFalse();
		}

		[TestMethod]
		public async Task CanMoveDiscDownGetter_IfNotLastDiscIsSelected_ReturnsTrue()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) },
				new DiscModel { Id = new ItemId("2"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 2) },
				new DiscModel { Id = new ItemId("3"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 3) },
			};

			var mocker = StubServices(new[] { adviseSet }, discs);
			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;
			target.SelectedAdviseSetDisc = discs[1];

			// Act

			var result = target.CanMoveDiscDown;

			// Assert

			result.Should().BeTrue();
		}

		[TestMethod]
		public async Task Load_IfSomeDiscsAreDeleted_LoadsOnlyActiveDiscs()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) },
				new DiscModel { Id = new ItemId("2"), AllSongs = new[] { new SongModel { DeleteDate = DateTimeOffset.Now } }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 2) },
				new DiscModel { Id = new ItemId("3"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 3) },
			};

			var mocker = StubServices(new[] { adviseSet }, discs);
			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			// Act

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;

			// Assert

			var availableDiscsViewModelMock = mocker.GetMock<IAvailableDiscsViewModel>();
			availableDiscsViewModelMock.Verify(x => x.LoadDiscs(new[] { discs[0], discs[2] }, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task CreateAdviseSetCommandHandler_IfAllDiscsBelongToSameAlbum_UsesAdviseSetNameWithAlbumTitle()
		{
			// Arrange

			var parentFolder = new ShallowFolderModel { Id = new ItemId("1"), Name = "Parent Folder" };

			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), Folder = parentFolder, AlbumTitle = "Some Album", AllSongs = new[] { new SongModel() } },
				new DiscModel { Id = new ItemId("2"), Folder = parentFolder, AlbumTitle = "Some Album", AllSongs = new[] { new SongModel() } },
			};

			var availableDiscsViewModelStub = new Mock<IAvailableDiscsViewModel>();
			availableDiscsViewModelStub.Setup(x => x.SelectedDiscs).Returns(discs);

			var mocker = StubServices(Array.Empty<AdviseSetModel>(), discs);
			mocker.Use(availableDiscsViewModelStub);

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);

			// Act

			target.CreateAdviseSetCommand.Execute(null);

			// Assert

			var adviseSetServiceMock = mocker.GetMock<IAdviseSetService>();

			Func<AdviseSetModel, bool> verifyAdviseSet = x => x.Name == "Parent Folder / Some Album";
			adviseSetServiceMock.Verify(x => x.CreateAdviseSet(It.Is<AdviseSetModel>(y => verifyAdviseSet(y)), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task CreateAdviseSetCommandHandler_IfDiscsBelongToDifferentAlbums_UsesCorrectDefaultAdviseSetName()
		{
			// Arrange

			var parentFolder = new ShallowFolderModel { Id = new ItemId("1"), Name = "Parent Folder" };

			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), Folder = parentFolder, AlbumTitle = "Some Album 1", AllSongs = new[] { new SongModel() } },
				new DiscModel { Id = new ItemId("2"), Folder = parentFolder, AlbumTitle = "Some Album 2", AllSongs = new[] { new SongModel() } },
			};

			var availableDiscsViewModelStub = new Mock<IAvailableDiscsViewModel>();
			availableDiscsViewModelStub.Setup(x => x.SelectedDiscs).Returns(discs);

			var mocker = StubServices(Array.Empty<AdviseSetModel>(), discs);
			mocker.Use(availableDiscsViewModelStub);

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);

			// Act

			target.CreateAdviseSetCommand.Execute(null);

			// Assert

			var adviseSetServiceMock = mocker.GetMock<IAdviseSetService>();

			Func<AdviseSetModel, bool> verifyAdviseSet = x => x.Name == "Parent Folder / New Advise Set";
			adviseSetServiceMock.Verify(x => x.CreateAdviseSet(It.Is<AdviseSetModel>(y => verifyAdviseSet(y)), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task CreateAdviseSetCommandHandler_IfDefaultAdviseSetNameIsAlreadyUsed_PicksNextFreeAdviseSetName()
		{
			// Arrange

			var existingAdviseSets = new[]
			{
				new AdviseSetModel { Id = new ItemId("1"), Name = "Parent Folder / New Advise Set" },
				new AdviseSetModel { Id = new ItemId("2"), Name = "Parent Folder / New Advise Set (2)" },
			};

			var parentFolder = new ShallowFolderModel { Id = new ItemId("1"), Name = "Parent Folder" };

			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), Folder = parentFolder, AlbumTitle = "Some Album 1", AllSongs = new[] { new SongModel() } },
				new DiscModel { Id = new ItemId("2"), Folder = parentFolder, AlbumTitle = "Some Album 2", AllSongs = new[] { new SongModel() } },
			};

			var availableDiscsViewModelStub = new Mock<IAvailableDiscsViewModel>();
			availableDiscsViewModelStub.Setup(x => x.SelectedDiscs).Returns(discs);

			var mocker = StubServices(existingAdviseSets, discs);
			mocker.Use(availableDiscsViewModelStub);

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);

			// Act

			target.CreateAdviseSetCommand.Execute(null);

			// Assert

			var adviseSetServiceMock = mocker.GetMock<IAdviseSetService>();

			Func<AdviseSetModel, bool> verifyAdviseSet = x => x.Name == "Parent Folder / New Advise Set (3)";
			adviseSetServiceMock.Verify(x => x.CreateAdviseSet(It.Is<AdviseSetModel>(y => verifyAdviseSet(y)), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task CreateAdviseSetCommandHandler_IfNoDiscsSelected_DoesNotAddAnyDiscsToAdviseSet()
		{
			// Arrange

			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() } },
				new DiscModel { Id = new ItemId("2"), AllSongs = new[] { new SongModel() } },
			};

			var availableDiscsViewModelStub = new Mock<IAvailableDiscsViewModel>();
			availableDiscsViewModelStub.Setup(x => x.SelectedDiscs).Returns(Enumerable.Empty<DiscModel>());

			var mocker = StubServices(Array.Empty<AdviseSetModel>(), discs);
			mocker.Use(availableDiscsViewModelStub);

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);

			// Act

			target.CreateAdviseSetCommand.Execute(null);

			// Assert

			var adviseSetServiceMock = mocker.GetMock<IAdviseSetService>();
			adviseSetServiceMock.Verify(x => x.AddDiscs(It.IsAny<AdviseSetModel>(), It.IsAny<IEnumerable<DiscModel>>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task CreateAdviseSetCommandHandler_IfSomeDiscsAreSelected_AddsSelectedDiscsToAdviseSet()
		{
			// Arrange

			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() } },
				new DiscModel { Id = new ItemId("2"), AllSongs = new[] { new SongModel() } },
			};

			var availableDiscsViewModelStub = new Mock<IAvailableDiscsViewModel>();
			availableDiscsViewModelStub.Setup(x => x.SelectedDiscs).Returns(discs);

			var mocker = StubServices(Array.Empty<AdviseSetModel>(), discs);
			mocker.Use(availableDiscsViewModelStub);

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);

			// Act

			target.CreateAdviseSetCommand.Execute(null);

			// Assert

			var adviseSetServiceMock = mocker.GetMock<IAdviseSetService>();
			adviseSetServiceMock.Verify(x => x.AddDiscs(It.IsAny<AdviseSetModel>(), discs, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task DeleteAdviseSetCommandHandler_IfDeletionIsNotConfirmed_DoesNotDeleteAdviseSet()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };

			var windowServiceStub = new Mock<IWindowService>();
			windowServiceStub.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ShowMessageBoxButton>(), It.IsAny<ShowMessageBoxIcon>())).Returns(ShowMessageBoxResult.No);

			var mocker = StubServices(new[] { adviseSet }, Array.Empty<DiscModel>());
			mocker.Use(windowServiceStub);

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;

			// Act

			target.DeleteAdviseSetCommand.Execute(null);

			// Assert

			var adviseSetServiceMock = mocker.GetMock<IAdviseSetService>();
			adviseSetServiceMock.Verify(x => x.DeleteAdviseSet(It.IsAny<AdviseSetModel>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task DeleteAdviseSetCommandHandler_IfDeletionIsConfirmed_DeleteAdviseSet()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };

			var windowServiceStub = new Mock<IWindowService>();
			windowServiceStub.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ShowMessageBoxButton>(), It.IsAny<ShowMessageBoxIcon>())).Returns(ShowMessageBoxResult.Yes);

			var mocker = StubServices(new[] { adviseSet }, Array.Empty<DiscModel>());
			mocker.Use(windowServiceStub);

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;

			// Act

			target.DeleteAdviseSetCommand.Execute(null);

			// Assert

			var adviseSetServiceMock = mocker.GetMock<IAdviseSetService>();
			adviseSetServiceMock.Verify(x => x.DeleteAdviseSet(adviseSet, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task DeleteAdviseSetCommandHandler_IfAdviseSetIsDeleted_ClearsAdviseSetForDiscs()
		{
			// Arrange

			var adviseSet1 = new AdviseSetModel { Id = new ItemId("1") };
			var adviseSet2 = new AdviseSetModel { Id = new ItemId("2") };

			var disc11 = new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet1, 1) };
			var disc12 = new DiscModel { Id = new ItemId("2"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet1, 2) };
			var disc21 = new DiscModel { Id = new ItemId("3"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet2, 1) };

			var discs = new[] { disc11, disc12, disc21 };

			var windowServiceStub = new Mock<IWindowService>();
			windowServiceStub.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ShowMessageBoxButton>(), It.IsAny<ShowMessageBoxIcon>())).Returns(ShowMessageBoxResult.Yes);

			var mocker = StubServices(new[] { adviseSet1, adviseSet2 }, discs);
			mocker.Use(windowServiceStub);

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet1;

			// Act

			target.DeleteAdviseSetCommand.Execute(null);

			// Assert

			disc11.AdviseSetInfo.Should().BeNull();
			disc12.AdviseSetInfo.Should().BeNull();
			disc21.AdviseSetInfo.Should().BeEquivalentTo(new AdviseSetInfo(adviseSet2, 1));
		}

		[TestMethod]
		public async Task MoveDiscUpCommandHandler_IfNotFirstDiscIsSelected_ReordersDiscsCorrectly()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) },
				new DiscModel { Id = new ItemId("2"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 2) },
				new DiscModel { Id = new ItemId("3"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 3) },
			};

			var mocker = StubServices(new[] { adviseSet }, discs);

			var adviseSetServiceMock = mocker.GetMock<IAdviseSetService>();
			adviseSetServiceMock.Setup(x => x.ReorderDiscs(adviseSet, new[] { discs[1], discs[0], discs[2] }, It.IsAny<CancellationToken>()))
				.Callback<AdviseSetModel, IEnumerable<DiscModel>, CancellationToken>((_, newDiscsOrder, _) =>
				{
					// Emulate logic of ReorderDiscs call for correct check of order for CurrentAdviseSetDiscs.
					var newDiscsOrderList = newDiscsOrder.ToList();
					newDiscsOrderList[0].AdviseSetInfo = new AdviseSetInfo(adviseSet, 1);
					newDiscsOrderList[1].AdviseSetInfo = new AdviseSetInfo(adviseSet, 2);
					newDiscsOrderList[2].AdviseSetInfo = new AdviseSetInfo(adviseSet, 3);
				});

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;
			target.SelectedAdviseSetDisc = discs[1];

			// Act

			target.MoveDiscUpCommand.Execute(null);

			// Assert

			adviseSetServiceMock.Verify(x => x.ReorderDiscs(adviseSet, new[] { discs[1], discs[0], discs[2] }, It.IsAny<CancellationToken>()), Times.Once);

			target.CurrentAdviseSetDiscs.Should().BeEquivalentTo(new[] { discs[1], discs[0], discs[2] }, x => x.WithStrictOrdering());
			target.SelectedAdviseSetDisc.Should().Be(discs[1]);
		}

		[TestMethod]
		public async Task MoveDiscDownCommandHandler_IfNotLastDiscIsSelected_ReordersDiscsCorrectly()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) },
				new DiscModel { Id = new ItemId("2"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 2) },
				new DiscModel { Id = new ItemId("3"), AllSongs = new[] { new SongModel() }, AdviseSetInfo = new AdviseSetInfo(adviseSet, 3) },
			};

			var mocker = StubServices(new[] { adviseSet }, discs);

			var adviseSetServiceMock = mocker.GetMock<IAdviseSetService>();
			adviseSetServiceMock.Setup(x => x.ReorderDiscs(adviseSet, new[] { discs[0], discs[2], discs[1] }, It.IsAny<CancellationToken>()))
				.Callback<AdviseSetModel, IEnumerable<DiscModel>, CancellationToken>((_, newDiscsOrder, _) =>
				{
					// Emulate logic of ReorderDiscs call for correct check of order for CurrentAdviseSetDiscs.
					var newDiscsOrderList = newDiscsOrder.ToList();
					newDiscsOrderList[0].AdviseSetInfo = new AdviseSetInfo(adviseSet, 1);
					newDiscsOrderList[1].AdviseSetInfo = new AdviseSetInfo(adviseSet, 2);
					newDiscsOrderList[2].AdviseSetInfo = new AdviseSetInfo(adviseSet, 3);
				});

			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			await target.Load(CancellationToken.None);
			target.SelectedAdviseSet = adviseSet;
			target.SelectedAdviseSetDisc = discs[1];

			// Act

			target.MoveDiscDownCommand.Execute(null);

			// Assert

			adviseSetServiceMock.Verify(x => x.ReorderDiscs(adviseSet, new[] { discs[0], discs[2], discs[1] }, It.IsAny<CancellationToken>()), Times.Once);

			target.CurrentAdviseSetDiscs.Should().BeEquivalentTo(new[] { discs[0], discs[2], discs[1] }, x => x.WithStrictOrdering());
			target.SelectedAdviseSetDisc.Should().Be(discs[1]);
		}

		[TestMethod]
		public void AvailableDiscsViewModelPropertyChangedEventHandler_ForSelectedItemsProperty_RaisesPropertyChangedEventForAffectedProperties()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseSetsEditorViewModel>();

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			mocker.GetMock<IAvailableDiscsViewModel>()
				.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(nameof(IAvailableDiscsViewModel.SelectedItems)));

			// Assert

			var expectedProperties = new[]
			{
				nameof(AdviseSetsEditorViewModel.CanCreateAdviseSet),
				nameof(AdviseSetsEditorViewModel.CanAddDiscs),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}

		private static AutoMocker StubServices(IReadOnlyCollection<AdviseSetModel> adviseSets, IReadOnlyCollection<DiscModel> discs)
		{
			var mocker = new AutoMocker();

			var adviseSetServiceStub = new Mock<IAdviseSetService>();
			adviseSetServiceStub.Setup(x => x.GetAllAdviseSets(It.IsAny<CancellationToken>())).ReturnsAsync(adviseSets);

			var discServiceStub = new Mock<IDiscsService>();
			discServiceStub.Setup(x => x.GetAllDiscs(It.IsAny<CancellationToken>())).ReturnsAsync(discs);

			mocker.Use(discServiceStub);
			mocker.Use(adviseSetServiceStub);

			return mocker;
		}
	}
}
