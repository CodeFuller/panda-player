using System.Threading.Tasks;

namespace CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary
{
	public interface ISongTagger
	{
		Task SetTagData(string songPath, SongTagData tagData);
	}
}
