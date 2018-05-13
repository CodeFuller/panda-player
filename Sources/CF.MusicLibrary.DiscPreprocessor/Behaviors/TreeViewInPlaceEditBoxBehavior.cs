using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using CF.MusicLibrary.DiscPreprocessor.Extensions;

namespace CF.MusicLibrary.DiscPreprocessor.Behaviors
{
	/// <remarks>
	/// Copy/paste from https://treeviewinplaceedit.codeplex.com
	/// </remarks>>
	public static class TreeViewInPlaceEditBoxBehavior
	{
		#region Attached Properties
		#region IsEditing
		public static readonly DependencyProperty IsEditingProperty = DependencyProperty.RegisterAttached(
		  "IsEditing", typeof(bool), typeof(TreeViewInPlaceEditBoxBehavior), new PropertyMetadata(OnIsEditingChanged));

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
		  "IsEditConfirmed", typeof(bool), typeof(TreeViewInPlaceEditBoxBehavior), new PropertyMetadata(OnIsEditConfirmedChanged));

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
		  "IsEditCanceled", typeof(bool), typeof(TreeViewInPlaceEditBoxBehavior), new PropertyMetadata(OnIsEditCanceledChanged));

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
		#endregion Attached Properties

		#region Private Methods
		private static void FocusAndSelect(TextBox textBox)
		{
			Keyboard.Focus(textBox);
			textBox.SelectAll();
		}
		#endregion

		#region Event Handlers
		private static void OnIsEditingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var textBox = obj as TextBox;
			if (textBox == null)
			{
				throw new ArgumentException("obj is not a TextBox");
			}

			textBox.Dispatcher.BeginInvoke((Action)(() => FocusAndSelect(textBox)), DispatcherPriority.Loaded);
		}

		private static void OnIsEditConfirmedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var textBox = obj as TextBox;
			if (textBox == null)
			{
				throw new ArgumentException("obj is not a TextBox");
			}

			if ((bool)args.NewValue && textBox.IsVisible)
			{
				textBox.UpdateBindingSource(TextBox.TextProperty);
			}
		}

		private static void OnIsEditCanceledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var textBox = obj as TextBox;
			if (textBox == null)
			{
				throw new ArgumentException("obj is not a TextBox");
			}

			if ((bool)args.NewValue && textBox.IsVisible)
			{
				textBox.UpdateBindingTarget(TextBox.TextProperty);
			}
		}
		#endregion Event Handlers
	}
}