namespace CF.MusicLibrary.AlbumPreprocessor.Extensions
{
	using System;
	using System.Windows;
	using System.Windows.Media;
	using Behaviors;

	/// <summary>
	/// Extension methods fo the DependencyObject objects
	/// </summary>
	/// <remarks>
	/// Copy/paste from https://treeviewinplaceedit.codeplex.com
	/// </remarks>>
	public static class DependencyObjectExtension
	{
		/// <summary>
		/// Find a sequence of children of type T
		/// </summary>
		public static T ParentOfType<T>(this DependencyObject control) where T : DependencyObject
		{
			return ParentOfType<T>(control, null);
		}

		/// <summary>
		/// Find a sequence of children of type T and apply filter if applicable
		/// </summary>
		public static T ParentOfType<T>(this DependencyObject control, Predicate<T> filter) where T : DependencyObject
		{
			var parent = VisualTreeHelper.GetParent(control);

			var t = parent as T;
			return t != null && (filter == null || filter(t))
					 ? t
					 : (parent != null ? parent.ParentOfType(filter) : null);
		}

		public static bool IsEditing(this DependencyObject obj)
		{
			return TreeViewInPlaceEditBehavior.GetIsEditing(obj);
		}

		public static void BeginEdit(this DependencyObject obj)
		{
			TreeViewInPlaceEditBehavior.SetIsEditing(obj, true);
		}

		public static void EndEdit(this DependencyObject obj, bool cancel)
		{
			TreeViewInPlaceEditBehavior.SetIsEditCanceled(obj, cancel);
			TreeViewInPlaceEditBehavior.SetIsEditConfirmed(obj, !cancel);
			TreeViewInPlaceEditBehavior.SetIsEditing(obj, false);
		}
	}
}
