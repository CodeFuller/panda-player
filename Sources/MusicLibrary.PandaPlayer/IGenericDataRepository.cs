namespace MusicLibrary.PandaPlayer
{
	public interface IGenericDataRepository<T>
		where T : class
	{
		void Save(T data);

		T Load();

		void Purge();
	}
}
