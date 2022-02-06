using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using PandaPlayer.DiscAdder.Views.Extensions;

namespace PandaPlayer.DiscAdder.Views.Behaviors
{
	// https://treeviewinplaceedit.codeplex.com
	internal static class TreeViewInPlaceEditBoxBehavior
	{
		public static readonly DependencyProperty IsEditingProperty = DependencyProperty.RegisterAttached(
		  "IsEditing", typeof(bool), typeof(TreeViewInPlaceEditBoxBehavior), new PropertyMetadata(OnIsEditingChanged));

		public static bool GetIsEditing(DependencyObject item)
		{
			return (bool)item.GetValue(IsEditingProperty);
		}

		public static void SetIsEditing(DependencyObject item, bool value)
		{
			item.SetValue(IsEditingProperty, value);
		}

		public static readonly DependencyProperty IsEditConfirmedProperty = DependencyProperty.RegisterAttached(
		  "IsEditConfirmed", typeof(bool), typeof(TreeViewInPlaceEditBoxBehavior), new PropertyMetadata(OnIsEditConfirmedChanged));

		public static bool GetIsEditConfirmed(DependencyObject item)
		{
			return (bool)item.GetValue(IsEditConfirmedProperty);
		}

		public static void SetIsEditConfirmed(DependencyObject item, bool value)
		{
			item.SetValue(IsEditConfirmedProperty, value);
		}

		public static readonly DependencyProperty IsEditCanceledProperty = DependencyProperty.RegisterAttached(
		  "IsEditCanceled", typeof(bool), typeof(TreeViewInPlaceEditBoxBehavior), new PropertyMetadata(OnIsEditCanceledChanged));

		public static bool GetIsEditCanceled(DependencyObject item)
		{
			return (bool)item.GetValue(IsEditCanceledProperty);
		}

		public static void SetIsEditCanceled(DependencyObject item, bool value)
		{
			item.SetValue(IsEditCanceledProperty, value);
		}

		private static void FocusAndSelect(TextBox textBox)
		{
			Keyboard.Focus(textBox);
			textBox.SelectAll();
		}

		private static void OnIsEditingChanged(DependencyObject item, DependencyPropertyChangedEventArgs args)
		{
			if (item is not TextBox textBox)
			{
				throw new ArgumentException($"{nameof(item)} should be a TextBox object");
			}

			textBox.Dispatcher.BeginInvoke(() => FocusAndSelect(textBox), DispatcherPriority.Loaded);
		}

		private static void OnIsEditConfirmedChanged(DependencyObject item, DependencyPropertyChangedEventArgs args)
		{
			if (item is not TextBox textBox)
			{
				throw new ArgumentException($"{nameof(item)} should be a TextBox object");
			}

			if ((bool)args.NewValue && textBox.IsVisible)
			{
				textBox.UpdateBindingSource(TextBox.TextProperty);
			}
		}

		private static void OnIsEditCanceledChanged(DependencyObject item, DependencyPropertyChangedEventArgs args)
		{
			if (item is not TextBox textBox)
			{
				throw new ArgumentException($"{nameof(item)} should be a TextBox object");
			}

			if ((bool)args.NewValue && textBox.IsVisible)
			{
				textBox.UpdateBindingTarget(TextBox.TextProperty);
			}
		}
	}
}
