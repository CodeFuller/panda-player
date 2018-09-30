using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using CF.MusicLibrary.DiscPreprocessor.Extensions;

namespace CF.MusicLibrary.DiscPreprocessor.Behaviors
{
	// Copy/paste from https://treeviewinplaceedit.codeplex.com
	public static class TreeViewInPlaceEditBoxBehavior
	{
		public static readonly DependencyProperty IsEditingProperty = DependencyProperty.RegisterAttached(
		  "IsEditing", typeof(bool), typeof(TreeViewInPlaceEditBoxBehavior), new PropertyMetadata(OnIsEditingChanged));

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
		  "IsEditConfirmed", typeof(bool), typeof(TreeViewInPlaceEditBoxBehavior), new PropertyMetadata(OnIsEditConfirmedChanged));

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
		  "IsEditCanceled", typeof(bool), typeof(TreeViewInPlaceEditBoxBehavior), new PropertyMetadata(OnIsEditCanceledChanged));

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

		private static void FocusAndSelect(TextBox textBox)
		{
			Keyboard.Focus(textBox);
			textBox.SelectAll();
		}

		private static void OnIsEditingChanged(DependencyObject item, DependencyPropertyChangedEventArgs args)
		{
			var textBox = item as TextBox;
			if (textBox == null)
			{
				throw new ArgumentException("obj is not a TextBox");
			}

			textBox.Dispatcher.BeginInvoke((Action)(() => FocusAndSelect(textBox)), DispatcherPriority.Loaded);
		}

		private static void OnIsEditConfirmedChanged(DependencyObject item, DependencyPropertyChangedEventArgs args)
		{
			var textBox = item as TextBox;
			if (textBox == null)
			{
				throw new ArgumentException("obj is not a TextBox");
			}

			if ((bool)args.NewValue && textBox.IsVisible)
			{
				textBox.UpdateBindingSource(TextBox.TextProperty);
			}
		}

		private static void OnIsEditCanceledChanged(DependencyObject item, DependencyPropertyChangedEventArgs args)
		{
			var textBox = item as TextBox;
			if (textBox == null)
			{
				throw new ArgumentException("obj is not a TextBox");
			}

			if ((bool)args.NewValue && textBox.IsVisible)
			{
				textBox.UpdateBindingTarget(TextBox.TextProperty);
			}
		}
	}
}
