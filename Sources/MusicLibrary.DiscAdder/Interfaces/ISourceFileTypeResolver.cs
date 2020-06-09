namespace MusicLibrary.DiscAdder.Interfaces
{
	internal interface ISourceFileTypeResolver
	{
		SourceFileType GetSourceFileType(string filePath);
	}
}
