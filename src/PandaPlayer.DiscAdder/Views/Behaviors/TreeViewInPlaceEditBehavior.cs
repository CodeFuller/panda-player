using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PandaPlayer.DiscAdder.Views.Extensions;

namespace PandaPlayer.DiscAdder.Views.Behaviors
{
	// https://treeviewinplaceedit.codeplex.com
	internal static class TreeViewInPlaceEditBehavior
	{
		public static readonly DependencyProperty IsEditableProperty = DependencyProperty.RegisterAttached(
			"IsEditable", typeof(bool), typeof(TreeViewInPlaceEditBehavior), new PropertyMetadata(OnIsEditableChanged));

		public static bool GetIsEditable(DependencyObject item)
		{
			_ = item ?? throw new ArgumentNullException(nameof(item));

			return (bool)item.GetValue(IsEditableProperty);
		}

		public static void SetIsEditable(DependencyObject item, bool value)
		{
			_ = item ?? throw new ArgumentNullException(nameof(item));

			item.SetValue(IsEditableProperty, value);
		}

		public static readonly DependencyProperty IsEditingProperty = DependencyProperty.RegisterAttached(
			"IsEditing", typeof(bool), typeof(TreeViewInPlaceEditBehavior));

		public static bool GetIsEditing(DependencyObject item)
		{
			_ = item ?? throw new ArgumentNullException(nameof(item));

			return (bool)item.GetValue(IsEditingProperty);
		}

		public static void SetIsEditing(DependencyObject item, bool value)
		{
			_ = item ?? throw new ArgumentNullException(nameof(item));

			item.SetValue(IsEditingProperty, value);
		}

		public static readonly DependencyProperty IsEditConfirmedProperty = DependencyProperty.RegisterAttached(
			"IsEditConfirmed", typeof(bool), typeof(TreeViewInPlaceEditBehavior));

		public static bool GetIsEditConfirmed(DependencyObject item)
		{
			_ = item ?? throw new ArgumentNullException(nameof(item));

			return (bool)item.GetValue(IsEditConfirmedProperty);
		}

		public static void SetIsEditConfirmed(DependencyObject item, bool value)
		{
			_ = item ?? throw new ArgumentNullException(nameof(item));

			item.SetValue(IsEditConfirmedProperty, value);
		}

		public static readonly DependencyProperty IsEditCanceledProperty = DependencyProperty.RegisterAttached(
			"IsEditCanceled", typeof(bool), typeof(TreeViewInPlaceEditBehavior));

		public static bool GetIsEditCanceled(DependencyObject item)
		{
			_ = item ?? throw new ArgumentNullException(nameof(item));

			return (bool)item.GetValue(IsEditCanceledProperty);
		}

		public static void SetIsEditCanceled(DependencyObject item, bool value)
		{
			_ = item ?? throw new ArgumentNullException(nameof(item));

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

		private static void OnIsEditableChanged(DependencyObject item, DependencyPropertyChangedEventArgs args)
		{
			if (item is not TreeView treeView)
			{
				throw new ArgumentException("TreeView object expected", nameof(item));
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
					treeView.EndEdit(cancel: true);
					break;

				case Key.Return:
					treeView.EndEdit(cancel: false);
					break;
			}
		}

		private static void TreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var treeView = (TreeView)sender;

			if (treeView.SelectedItem != GetLastSelectedItem(treeView))
			{
				SetLastSelectedItem(treeView, treeView.SelectedItem);
				treeView.EndEdit(cancel: true);
			}
		}

		private static void TreeViewPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			var treeView = (TreeView)sender;
			var element = (UIElement)e.OriginalSource;

			var selectedItem = element.ParentOfType<TreeViewItem>();
			if (selectedItem == null)
			{
				// Cancelling edit due to click on nowhere.
				treeView.EndEdit(cancel: true);
			}
		}
	}
}
