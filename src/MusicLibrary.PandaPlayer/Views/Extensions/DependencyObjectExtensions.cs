using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MusicLibrary.PandaPlayer.Views.Extensions
{
	internal static class DependencyObjectExtensions
	{
		// Checks whether any of child controls have validation errors.
		// https://stackoverflow.com/a/4650392/5740031
		public static bool IsValid(this DependencyObject obj)
		{
			return !Validation.GetHasError(obj) &&
			       LogicalTreeHelper.GetChildren(obj).OfType<DependencyObject>().All(IsValid);
		}
	}
}
