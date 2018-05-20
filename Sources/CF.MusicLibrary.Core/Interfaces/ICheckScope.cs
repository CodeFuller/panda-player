using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.Core.Interfaces
{
	public interface ICheckScope
	{
		bool Contains(Disc disc);

		bool Contains(Song song);
	}
}
