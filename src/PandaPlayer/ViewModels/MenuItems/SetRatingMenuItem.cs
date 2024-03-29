using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Views.Extensions;

namespace PandaPlayer.ViewModels.MenuItems
{
	public class SetRatingMenuItem : CommandMenuItem
	{
		private readonly RatingModel rating;

		public SetRatingMenuItem(RatingModel rating, Func<Task> commandAction)
			: base(commandAction)
		{
			this.rating = rating;
		}

		public override MenuItem GetMenuItemControl()
		{
			var menuItem = base.GetMenuItemControl();

			var ratingImageFileName = rating.ToRatingImageFileName();
			var ratingImage = new BitmapImage(new Uri($"pack://application:,,,/PandaPlayer;component{ratingImageFileName}", UriKind.Absolute));

			// https://stackoverflow.com/a/248638/5740031
			var image = new FrameworkElementFactory(typeof(Image));
			image.SetValue(FrameworkElement.HeightProperty, 15d);
			image.SetValue(Image.SourceProperty, ratingImage);

			menuItem.HeaderTemplate = new DataTemplate
			{
				VisualTree = image,
			};

			return menuItem;
		}
	}
}
