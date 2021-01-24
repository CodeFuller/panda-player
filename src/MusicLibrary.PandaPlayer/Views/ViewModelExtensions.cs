using System;

namespace MusicLibrary.PandaPlayer.Views
{
	public static class ViewModelExtensions
	{
		public static TViewModel GetViewModel<TViewModel>(this Object dataContext)
			where TViewModel : class
		{
			if (dataContext == null)
			{
				throw new ArgumentNullException($"DataContext for {typeof(TViewModel)} is null");
			}

			if (!(dataContext is TViewModel viewModel))
			{
				throw new InvalidOperationException($"Unexpected type of DataContext: Expected {typeof(TViewModel)}, actual is {dataContext.GetType()}");
			}

			return viewModel;
		}
	}
}
