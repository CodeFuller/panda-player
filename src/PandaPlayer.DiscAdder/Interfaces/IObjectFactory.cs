namespace PandaPlayer.DiscAdder.Interfaces
{
	internal interface IObjectFactory<out TType>
	{
		TType CreateInstance();
	}
}
