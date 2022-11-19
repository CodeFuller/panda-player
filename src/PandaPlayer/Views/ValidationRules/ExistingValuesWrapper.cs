using System.Collections.Generic;
using System.Windows;

namespace PandaPlayer.Views.ValidationRules
{
	// https://stackoverflow.com/questions/46519802/
	// https://social.technet.microsoft.com/wiki/contents/articles/31422.wpf-passing-a-data-bound-value-to-a-validation-rule.aspx
	public class ExistingValuesWrapper : DependencyObject
	{
		public static readonly DependencyProperty ValueTitleProperty = DependencyProperty.Register(
			nameof(ValueTitle), typeof(string), typeof(ExistingValuesWrapper), new FrameworkPropertyMetadata("Value"));

		public static readonly DependencyProperty ExistingValuesProperty = DependencyProperty.Register(
			nameof(ExistingValues), typeof(IReadOnlyCollection<string>), typeof(ExistingValuesWrapper), new FrameworkPropertyMetadata(new List<string>()));

		public string ValueTitle
		{
			get => (string)GetValue(ValueTitleProperty);
			set => SetValue(ValueTitleProperty, value);
		}

		public IReadOnlyCollection<string> ExistingValues
		{
			get => (IReadOnlyCollection<string>)GetValue(ExistingValuesProperty);
			set => SetValue(ExistingValuesProperty, value);
		}
	}
}
