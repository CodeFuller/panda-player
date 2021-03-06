﻿using System;

namespace PandaPlayer.Views
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

			if (dataContext is not TViewModel viewModel)
			{
				throw new InvalidOperationException($"Unexpected type of DataContext: Expected {typeof(TViewModel)}, actual is {dataContext.GetType()}");
			}

			return viewModel;
		}
	}
}
