namespace MusicLibrary.Services.Interfaces
{
	public interface IOperationProgress
	{
		void SetOperationCost(int cost);

		void IncrementOperationProgress();
	}
}
