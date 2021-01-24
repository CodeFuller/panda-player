using System;

namespace MusicLibrary.Services.Interfaces
{
	public interface IClock
	{
		DateTimeOffset Now { get; }
	}
}
