using System.Windows;

namespace PandaPlayer.DiscAdder.Views.Extensions
{
	// https://treeviewinplaceedit.codeplex.com
	internal static class FrameworkElementExtension
	{
		public static void UpdateBindingTarget(this FrameworkElement element, DependencyProperty property)
		{
			element.GetBindingExpression(property)?.UpdateTarget();
		}

		public static void UpdateBindingSource(this FrameworkElement element, DependencyProperty property)
		{
			element.GetBindingExpression(property)?.UpdateSource();
		}
	}
}
