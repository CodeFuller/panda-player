using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CF.MusicLibrary.DiscPreprocessor.Extensions;

namespace CF.MusicLibrary.DiscPreprocessor.Behaviors
{
	/// <remarks>
	/// Copy/paste from https://treeviewinplaceedit.codeplex.com
	/// </remarks>>
	public static class TreeViewInPlaceEditBehavior
	{
		#region Attached Properties
		#region IsEditable
		public static readonly DependencyProperty IsEditableProperty = DependencyProperty.RegisterAttached(
		  "IsEditable", typeof(bool), typeof(TreeViewInPlaceEditBehavior), new PropertyMetadata(OnIsEditableChanged));

		public static bool GetIsEditable(DependencyObject obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}
			return (bool)obj.GetValue(IsEditableProperty);
		}

		public static void SetIsEditable(DependencyObject obj, bool value)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}
			obj.SetValue(IsEditableProperty, value);
		}
		#endregion IsEditable

		#region IsEditing
		public static readonly DependencyProperty IsEditingProperty = DependencyProperty.RegisterAttached(
		  "IsEditing", typeof(bool), typeof(TreeViewInPlaceEditBehavior));

		public static bool GetIsEditing(DependencyObject obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}
			return (bool)obj.GetValue(IsEditingProperty);
		}

		public static void SetIsEditing(DependencyObject obj, bool value)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}
			obj.SetValue(IsEditingProperty, value);
		}
		#endregion IsEditing

		#region IsEditConfirmed
		public static readonly DependencyProperty IsEditConfirmedProperty = DependencyProperty.RegisterAttached(
		  "IsEditConfirmed", typeof(bool), typeof(TreeViewInPlaceEditBehavior));

		public static bool GetIsEditConfirmed(DependencyObject obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}
			return (bool)obj.GetValue(IsEditConfirmedProperty);
		}

		public static void SetIsEditConfirmed(DependencyObject obj, bool value)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}
			obj.SetValue(IsEditConfirmedProperty, value);
		}
		#endregion IsEditConfirmed

		#region IsEditCanceled
		public static readonly DependencyProperty IsEditCanceledProperty = DependencyProperty.RegisterAttached(
		  "IsEditCanceled", typeof(bool), typeof(TreeViewInPlaceEditBehavior));

		public static bool GetIsEditCanceled(DependencyObject obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}
			return (bool)obj.GetValue(IsEditCanceledProperty);
		}

		public static void SetIsEditCanceled(DependencyObject obj, bool value)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}
			obj.SetValue(IsEditCanceledProperty, value);
		}
		#endregion IsEditCanceled

		#region LastSelectedItem
		private static readonly DependencyProperty LastSelectedItemProperty = DependencyProperty.RegisterAttached(
		  "LastSelectedItem", typeof(object), typeof(TreeViewInPlaceEditBehavior));

		private static object GetLastSelectedItem(DependencyObject obj)
		{
			return obj.GetValue(LastSelectedItemProperty);
		}

		private static void SetLastSelectedItem(DependencyObject obj, object value)
		{
			obj.SetValue(LastSelectedItemProperty, value);
		}
		#endregion LastSelectedItem

		#region LastSelectedTime
		private static readonly DependencyProperty LastSelectedTimeProperty = DependencyProperty.RegisterAttached(
		  "LastSelectedTime", typeof(DateTime), typeof(TreeViewInPlaceEditBehavior));

		private static DateTime GetLastSelectedTime(DependencyObject obj)
		{
			return (DateTime)obj.GetValue(LastSelectedTimeProperty);
		}

		private static void SetLastSelectedTime(DependencyObject obj, DateTime value)
		{
			obj.SetValue(LastSelectedTimeProperty, value);
		}
		#endregion LastSelectedTime
		#endregion Attached Properties

		#region Event Handlers
		private static void OnIsEditableChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var treeView = obj as TreeView;
			if (treeView == null)
			{
				throw new ArgumentException("obj is not a TreeView");
			}

			treeView.PreviewKeyDown -= TreeViewPreviewKeyDown;
			treeView.PreviewMouseLeftButtonUp -= TreeViewPreviewMouseLeftButtonUp;
			treeView.SelectedItemChanged -= TreeViewSelectedItemChanged;
			if ((bool)args.NewValue)
			{
				treeView.PreviewKeyDown += TreeViewPreviewKeyDown;
				treeView.PreviewMouseLeftButtonUp += TreeViewPreviewMouseLeftButtonUp;
				treeView.SelectedItemChanged += TreeViewSelectedItemChanged;
			}
		}

		private static void TreeViewPreviewKeyDown(object sender, KeyEventArgs e)
		{
			var treeView = (TreeView)sender;
			switch (e.Key)
			{
				case Key.F2:
					treeView.BeginEdit();
					break;
				case Key.Escape:
					if (treeView.IsEditing())
					{
						treeView.EndEdit(true);
					}
					break;
				case Key.Return:
					if (treeView.IsEditing())
					{
						treeView.EndEdit(false);
					}
					break;
			}
		}

		private static void TreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var treeView = (TreeView)sender;
			var lastSelectedItem = GetLastSelectedItem(treeView);
			if (lastSelectedItem != treeView.SelectedItem)
			{
				////Selection changed, let's save the selected item and the selected time
				SetLastSelectedItem(treeView, treeView.SelectedItem);
				SetLastSelectedTime(treeView, DateTime.Now);
				treeView.EndEdit(true);
			}
		}

		private static void TreeViewPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			var treeView = (TreeView)sender;
			var element = (UIElement)e.OriginalSource;

			var selectedItem = element.ParentOfType<TreeViewItem>();
			if (selectedItem == null)
			{
				////We're clicking on nowhere, let's cancel the editing
				treeView.EndEdit(true);
				return;
			}

			var lastSelectedItem = GetLastSelectedItem(treeView);
			if (lastSelectedItem == null || lastSelectedItem != treeView.SelectedItem)
			{
				return;
			}

			var lastSelctedTime = GetLastSelectedTime(treeView);
			var interval = DateTime.Now.Subtract(lastSelctedTime).TotalMilliseconds;
			if (interval >= 400 && interval <= 1200)
			{
				////It's long double click, consider it as a edit sign 
				treeView.BeginEdit();
			}
		}
		#endregion Event Handlers
	}
}
