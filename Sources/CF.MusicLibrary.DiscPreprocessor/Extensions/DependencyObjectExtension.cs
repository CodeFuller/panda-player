using System;
using System.Windows;
using System.Windows.Media;
using CF.MusicLibrary.DiscPreprocessor.Behaviors;

namespace CF.MusicLibrary.DiscPreprocessor.Extensions
{
	// Copy/paste from https://treeviewinplaceedit.codeplex.com
	public static class DependencyObjectExtension
	{
		/// <summary>
		/// Find a sequence of children of type T
		/// </summary>
		/// <typeparam name="T">Type of control to search.</typeparam>
		/// <param name="control">Child control.</param>
		/// <returns>
		/// First parent control of type T.
		/// </returns>
		public static T ParentOfType<T>(this DependencyObject control) where T : DependencyObject
		{
			return ParentOfType<T>(control, null);
		}

		/// <summary>
		/// Find a sequence of children of type T and apply filter if applicable
		/// </summary>
		/// <typeparam name="T">Type of control to search.</typeparam>
		/// <param name="control">Child control.</param>
		/// <param name="filter">Filter that should be met.</param>
		/// <returns>
		/// First parent control of type T that satisfies given filter.
		/// </returns>
		public static T ParentOfType<T>(this DependencyObject control, Predicate<T> filter) where T : DependencyObject
		{
			var parent = VisualTreeHelper.GetParent(control);

			var t = parent as T;
			return t != null && (filter == null || filter(t))
					 ? t
					 : (parent != null ? parent.ParentOfType(filter) : null);
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
	}
}
