using System;
using CommunityToolkit.Mvvm.Messaging;
using Moq.AutoMock;

namespace PandaPlayer.UnitTests.Helpers
{
	internal static class AutoMockerExtensions
	{
		public static IMessenger StubMessenger(this AutoMocker mocker)
		{
			var messenger = new WeakReferenceMessenger();

			mocker.Use<IMessenger>(messenger);

			return messenger;
		}

		public static void SendMessage<TEvent>(this AutoMocker mocker, TEvent e)
			where TEvent : EventArgs
		{
			mocker.Get<IMessenger>().Send(e);
		}
	}
}
