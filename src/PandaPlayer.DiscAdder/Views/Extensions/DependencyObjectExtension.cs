using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PandaPlayer.DiscAdder.Views.Behaviors;

namespace PandaPlayer.DiscAdder.Views.Extensions
{
	// https://treeviewinplaceedit.codeplex.com
	internal static class DependencyObjectExtension
	{
		public static T ParentOfType<T>(this DependencyObject control)
			where T : DependencyObject
		{
			while (control != null)
			{
				var parent = VisualTreeHelper.GetParent(control);

				if (parent is T typedParent)
				{
					return typedParent;
				}

				control = parent;
			}

			return null;
		}

		public static bool IsEditing(this DependencyObject item)
		{
			return TreeViewInPlaceEditBehavior.GetIsEditing(item);
		}

		public static void BeginEdit(this DependencyObject item)
		{
			TreeViewInPlaceEditBehavior.SetIsEditing(item, true);
		}

		public static void EndEdit(this DependencyObject item, bool cancel)
		{
			TreeViewInPlaceEditBehavior.SetIsEditCanceled(item, cancel);
			TreeViewInPlaceEditBehavior.SetIsEditConfirmed(item, !cancel);
			TreeViewInPlaceEditBehavior.SetIsEditing(item, false);
		}

		public static ScrollViewer FindScrollViewer(this DependencyObject control)
		{
			var scrollViewer = control as ScrollViewer;

			for (var i = 0; scrollViewer == null && i < VisualTreeHelper.GetChildrenCount(control); ++i)
			{
				var child = VisualTreeHelper.GetChild(control, i);
				scrollViewer = FindScrollViewer(child);
			}

			return scrollViewer;
		}
	}
}
