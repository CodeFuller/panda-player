namespace PandaPlayer.DiscAdder.Interfaces
{
	internal interface ISourceFileTypeResolver
	{
		SourceFileType GetSourceFileType(string filePath);
	}
}
