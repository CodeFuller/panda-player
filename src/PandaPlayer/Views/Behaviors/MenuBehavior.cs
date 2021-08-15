using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.Views.Behaviors
{
	// https://stackoverflow.com/questions/4823760/
	public static class MenuBehavior
	{
		public static readonly DependencyProperty MenuItemsProperty =
			DependencyProperty.RegisterAttached("MenuItems", typeof(IEnumerable<BasicMenuItem>), typeof(MenuBehavior), new FrameworkPropertyMetadata(MenuItemsChanged));

		public static IEnumerable<BasicMenuItem> GetMenuItems(DependencyObject element)
		{
			_ = element ?? throw new ArgumentNullException(nameof(element));

			return (IEnumerable<BasicMenuItem>)element.GetValue(MenuItemsProperty);
		}

		public static void SetMenuItems(DependencyObject element, IEnumerable<BasicMenuItem> value)
		{
			_ = element ?? throw new ArgumentNullException(nameof(element));

			element.SetValue(MenuItemsProperty, value);
		}

		private static void MenuItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var menu = (MenuItem)d;

			if (e.OldValue != e.NewValue)
			{
				menu.ItemsSource = ConvertViewModelsToFrameworkElements((IEnumerable<BasicMenuItem>)e.NewValue);
			}
		}

		private static IEnumerable<FrameworkElement> ConvertViewModelsToFrameworkElements(IEnumerable<BasicMenuItem> viewModels)
		{
			var frameworkElementList = new List<FrameworkElement>();

			foreach (var viewModel in viewModels)
			{
				switch (viewModel)
				{
					case NormalMenuItem mi:
						frameworkElementList.Add(new MenuItem
						{
							Header = mi.Header,
							IsCheckable = mi.IsCheckable,
							IsChecked = mi.IsChecked,
							Command = mi.Command,
						});
						break;

					case SeparatorMenuItem:
						frameworkElementList.Add(new Separator());
						break;
				}
			}

			return frameworkElementList;
		}
	}
}
