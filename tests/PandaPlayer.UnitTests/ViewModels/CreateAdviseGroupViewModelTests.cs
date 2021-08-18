using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.ViewModels;

namespace PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class CreateAdviseGroupViewModelTests
	{
		[TestMethod]
		public void Load_FillsDataCorrectly()
		{
			// Arrange

			var existingAdviseGroupNames = new[]
			{
				"Existing Advise Group Name 1",
				"Existing Advise Group Name 2",
			};

			var target = new CreateAdviseGroupViewModel();

			// Act

			target.Load("Initial Advise Group Name", existingAdviseGroupNames);

			// Assert

			target.AdviseGroupName.Should().Be("Initial Advise Group Name");
			target.ExistingAdviseGroupNames.Should().BeEquivalentTo(existingAdviseGroupNames, x => x.WithStrictOrdering());
		}
	}
}
