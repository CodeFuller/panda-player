using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PandaPlayer.DiscAdder.Views.Extensions;

namespace PandaPlayer.DiscAdder.Views.Behaviors
{
	// Copy/paste from https://treeviewinplaceedit.codeplex.com
	internal static class TreeViewInPlaceEditBehavior
	{
		public static readonly DependencyProperty IsEditableProperty = DependencyProperty.RegisterAttached(
		  "IsEditable", typeof(bool), typeof(TreeViewInPlaceEditBehavior), new PropertyMetadata(OnIsEditableChanged));

		public static bool GetIsEditable(DependencyObject item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			return (bool)item.GetValue(IsEditableProperty);
		}

		public static void SetIsEditable(DependencyObject item, bool value)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			item.SetValue(IsEditableProperty, value);
		}

		public static readonly DependencyProperty IsEditingProperty = DependencyProperty.RegisterAttached(
		  "IsEditing", typeof(bool), typeof(TreeViewInPlaceEditBehavior));

		public static bool GetIsEditing(DependencyObject item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			return (bool)item.GetValue(IsEditingProperty);
		}

		public static void SetIsEditing(DependencyObject item, bool value)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			item.SetValue(IsEditingProperty, value);
		}

		public static readonly DependencyProperty IsEditConfirmedProperty = DependencyProperty.RegisterAttached(
		  "IsEditConfirmed", typeof(bool), typeof(TreeViewInPlaceEditBehavior));

		public static bool GetIsEditConfirmed(DependencyObject item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			return (bool)item.GetValue(IsEditConfirmedProperty);
		}

		public static void SetIsEditConfirmed(DependencyObject item, bool value)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			item.SetValue(IsEditConfirmedProperty, value);
		}

		public static readonly DependencyProperty IsEditCanceledProperty = DependencyProperty.RegisterAttached(
		  "IsEditCanceled", typeof(bool), typeof(TreeViewInPlaceEditBehavior));

		public static bool GetIsEditCanceled(DependencyObject item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			return (bool)item.GetValue(IsEditCanceledProperty);
		}

		public static void SetIsEditCanceled(DependencyObject item, bool value)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			item.SetValue(IsEditCanceledProperty, value);
		}

		private static readonly DependencyProperty LastSelectedItemProperty = DependencyProperty.RegisterAttached(
		  "LastSelectedItem", typeof(object), typeof(TreeViewInPlaceEditBehavior));

		private static object GetLastSelectedItem(DependencyObject item)
		{
			return item.GetValue(LastSelectedItemProperty);
		}

		private static void SetLastSelectedItem(DependencyObject item, object value)
		{
			item.SetValue(LastSelectedItemProperty, value);
		}

		private static readonly DependencyProperty LastSelectedTimeProperty = DependencyProperty.RegisterAttached(
		  "LastSelectedTime", typeof(DateTime), typeof(TreeViewInPlaceEditBehavior));

		private static DateTime GetLastSelectedTime(DependencyObject item)
		{
			return (DateTime)item.GetValue(LastSelectedTimeProperty);
		}

		private static void SetLastSelectedTime(DependencyObject item, DateTime value)
		{
			item.SetValue(LastSelectedTimeProperty, value);
		}

		private static void OnIsEditableChanged(DependencyObject item, DependencyPropertyChangedEventArgs args)
		{
			var treeView = item as TreeView;
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
				// Selection changed, let's save the selected item and the selected time
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
				// We're clicking on nowhere, let's cancel the editing
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
				// It's long double click, consider it as a edit sign.
				treeView.BeginEdit();
			}
		}
	}
}
