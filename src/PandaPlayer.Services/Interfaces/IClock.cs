using System;

namespace PandaPlayer.Services.Interfaces
{
	public interface IClock
	{
		DateTimeOffset Now { get; }
	}
}
