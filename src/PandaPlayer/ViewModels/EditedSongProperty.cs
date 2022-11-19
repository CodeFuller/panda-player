using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace PandaPlayer.ViewModels
{
	public class EditedSongProperty<T> : ViewModelBase
	{
		private bool hasValue;

		public bool HasValue
		{
			get => hasValue;
			private set => Set(ref hasValue, value);
		}

		public bool HasBlankValue => HasValue && Value == null;

		private T propertyValue;

		public T Value
		{
			get
			{
				if (!HasValue)
				{
					throw new InvalidOperationException("Song property is not set");
				}

				return propertyValue;
			}

			set
			{
				Set(ref propertyValue, value);
				HasValue = true;
			}
		}

		public EditedSongProperty()
		{
			HasValue = false;
		}

		public EditedSongProperty(T initialValue)
		{
			Value = initialValue;
		}

		public override string ToString()
		{
			if (!HasValue)
			{
				return "< keep >";
			}

			return Value == null ? "< blank >" : Value.ToString();
		}

		public bool Equals(EditedSongProperty<T> cmp, IEqualityComparer<T> comparer)
		{
			if (HasValue != cmp.HasValue)
			{
				return false;
			}

			if (!HasValue)
			{
				return true;
			}

			return comparer.Equals(Value, cmp.Value);
		}

		public override bool Equals(Object obj)
		{
			if (!(obj is EditedSongProperty<T> cmp))
			{
				return false;
			}

			return Equals(cmp, EqualityComparer<T>.Default);
		}

		public override int GetHashCode()
		{
			// Overflow is fine, just wrap
			unchecked
			{
				var hash = 17;
				hash = (hash * 23) + HasValue.GetHashCode();
				if (HasValue && Value != null)
				{
					hash = (hash * 23) + Value.GetHashCode();
				}

				return hash;
			}
		}
	}
}
