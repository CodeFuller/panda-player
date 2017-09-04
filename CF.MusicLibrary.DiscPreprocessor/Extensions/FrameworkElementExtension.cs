using System;
using System.Windows;

namespace CF.MusicLibrary.DiscPreprocessor.Extensions
{
	/// <remarks>
	/// Copy/paste from https://treeviewinplaceedit.codeplex.com
	/// </remarks>>
	public static class FrameworkElementExtension
	{
		public static void UpdateBindingTarget(this FrameworkElement element, DependencyProperty property)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			element.GetBindingExpression(property)?.UpdateTarget();
		}

		public static void UpdateBindingSource(this FrameworkElement element, DependencyProperty property)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			element.GetBindingExpression(property)?.UpdateSource();
		}
	}
}
