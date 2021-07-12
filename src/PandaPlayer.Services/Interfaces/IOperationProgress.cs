namespace PandaPlayer.Services.Interfaces
{
	public interface IOperationProgress
	{
		void SetOperationCost(int cost);

		void IncrementOperationProgress();
	}
}
