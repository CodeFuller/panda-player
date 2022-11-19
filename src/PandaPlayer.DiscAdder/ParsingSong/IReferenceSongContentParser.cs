namespace PandaPlayer.DiscAdder.ParsingSong
{
	internal interface IReferenceSongContentParser
	{
		ReferenceSongContent Parse(int trackNumber, string rawReferenceSongContent);
	}
}
