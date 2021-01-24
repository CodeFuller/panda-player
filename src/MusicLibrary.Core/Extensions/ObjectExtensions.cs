using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MusicLibrary.Core.Extensions
{
	internal static class ObjectExtensions
	{
		public static void SetField<T>(this Object sender, PropertyChangedEventHandler handler, ref T field, T newValue, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, newValue))
			{
				return;
			}

			field = newValue;

			handler?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
		}
	}
}
