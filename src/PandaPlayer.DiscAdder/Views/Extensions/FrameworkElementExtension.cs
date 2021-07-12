using System;
using System.Windows;

namespace PandaPlayer.DiscAdder.Views.Extensions
{
	// Copy/paste from https://treeviewinplaceedit.codeplex.com
	internal static class FrameworkElementExtension
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
