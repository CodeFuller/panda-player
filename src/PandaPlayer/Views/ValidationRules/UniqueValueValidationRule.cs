using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace PandaPlayer.Views.ValidationRules
{
	public class UniqueValueValidationRule : ValidationRule
	{
		public ExistingValuesWrapper ExistingValuesWrapper { get; set; }

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			var stringValue = value as string;
			if (ExistingValuesWrapper.ExistingValues.Any(x => String.Equals(x, stringValue, StringComparison.Ordinal)))
			{
				return new ValidationResult(false, $"{ExistingValuesWrapper.ValueTitle} '{stringValue}' already exists");
			}

			return new ValidationResult(true, null);
		}
	}
}
