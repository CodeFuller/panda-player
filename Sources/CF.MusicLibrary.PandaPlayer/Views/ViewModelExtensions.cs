using System;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.PandaPlayer.Views
{
	public static class ViewModelExtensions
	{
		public static TViewModel GetViewModel<TViewModel>(this Object dataContext) where TViewModel : class
		{
			if (dataContext == null)
			{
				throw new ArgumentNullException(Current($"DataContext for {typeof(TViewModel)} is null"));
			}

			var viewModel = dataContext as TViewModel;
			if (viewModel == null)
			{
				throw new InvalidOperationException(Current($"Unexpected type of DataContext: Expected {typeof(TViewModel)}, actual is {dataContext.GetType()}"));
			}

			return viewModel;
		}
	}
}
