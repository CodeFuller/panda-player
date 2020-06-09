namespace MusicLibrary.DiscAdder.Interfaces
{
	public interface ISourceFileTypeResolver
	{
		SourceFileType GetSourceFileType(string filePath);
	}
}
