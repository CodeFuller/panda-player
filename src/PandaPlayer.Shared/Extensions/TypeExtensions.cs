using System;
using System.ComponentModel;
using System.Linq;

namespace PandaPlayer.Shared.Extensions
{
	internal static class TypeExtensions
	{
		// http://stackoverflow.com/a/479417/5740031
		public static string GetDescription<T>(this T enumerationValue)
			where T : struct
		{
			var type = enumerationValue.GetType();
			if (!type.IsEnum)
			{
				throw new ArgumentException("EnumerationValue must be of Enum type", nameof(enumerationValue));
			}

			// Tries to find a DescriptionAttribute for a potential friendly name for the enum
			var memberInfo = type.GetMember(enumerationValue.ToString());
			if (memberInfo.Any())
			{
				var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

				if (attrs.Any())
				{
					// Pull out the description value
					return ((DescriptionAttribute)attrs.First()).Description;
				}
			}

			// If we have no description attribute, just return the ToString of the enum
			return enumerationValue.ToString();
		}
	}
}
