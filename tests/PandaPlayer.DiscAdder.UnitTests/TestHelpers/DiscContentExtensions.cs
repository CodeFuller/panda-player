using CommunityToolkit.Mvvm.Messaging;
using Moq;
using PandaPlayer.DiscAdder.ViewModels.SourceContent;

namespace PandaPlayer.DiscAdder.UnitTests.TestHelpers
{
	internal static class DiscContentExtensions
	{
		public static ReferenceDiscTreeItem ToReferenceDiscTreeItem(this ReferenceDiscContent disc)
		{
			return new ReferenceDiscTreeItem(disc);
		}

		public static ActualDiscTreeItem ToActualDiscTreeItem(this ActualDiscContent disc)
		{
			return new ActualDiscTreeItem(disc, Mock.Of<IMessenger>());
		}
	}
}
