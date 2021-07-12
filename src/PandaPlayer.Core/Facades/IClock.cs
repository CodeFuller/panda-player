using System;

namespace PandaPlayer.Core.Facades
{
	public interface IClock
	{
		DateTimeOffset Now { get; }
	}
}
