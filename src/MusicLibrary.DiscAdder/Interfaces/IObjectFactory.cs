namespace MusicLibrary.DiscAdder.Interfaces
{
	internal interface IObjectFactory<out TType>
	{
		TType CreateInstance();
	}
}
