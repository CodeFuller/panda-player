using MusicLibrary.Core.Objects;

namespace MusicLibrary.Core.Interfaces
{
	public interface ICheckScope
	{
		bool Contains(Disc disc);

		bool Contains(Song song);
	}
}
