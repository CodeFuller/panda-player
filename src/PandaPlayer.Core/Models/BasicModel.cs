using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PandaPlayer.Core.Models
{
	public class BasicModel
	{
		private ItemId id;

		public ItemId Id
		{
			get => id;
			set
			{
				if (id != null)
				{
					throw new InvalidOperationException("Can not overwrite model id");
				}

				id = value;
			}
		}

		protected void SetField<T>(PropertyChangedEventHandler handler, ref T field, T newValue, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, newValue))
			{
				return;
			}

			field = newValue;

			handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
