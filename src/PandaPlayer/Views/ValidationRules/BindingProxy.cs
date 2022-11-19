using System.Windows;

namespace PandaPlayer.Views.ValidationRules
{
	public class BindingProxy : Freezable
	{
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new PropertyMetadata(null));

		protected override Freezable CreateInstanceCore()
		{
			return new BindingProxy();
		}

		public object Data
		{
			get => GetValue(DataProperty);
			set => SetValue(DataProperty, value);
		}
	}
}
