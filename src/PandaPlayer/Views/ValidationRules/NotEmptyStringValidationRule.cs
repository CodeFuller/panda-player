using System;
using System.Globalization;
using System.Windows.Controls;

namespace PandaPlayer.Views.ValidationRules
{
	public class NotEmptyStringValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			var stringValue = value as string;
			return String.IsNullOrEmpty(stringValue) ? new ValidationResult(false, "Please enter non-empty string.") : new ValidationResult(true, null);
		}
	}
}
