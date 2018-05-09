namespace CF.MusicLibrary.DiscPreprocessor.Interfaces
{
	public interface ISourceFileTypeResolver
	{
		SourceFileType GetSourceFileType(string filePath);
	}
}
