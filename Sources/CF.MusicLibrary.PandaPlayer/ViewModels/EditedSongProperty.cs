using System;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class EditedSongProperty<T> : ViewModelBase
	{
		private bool hasValue;

		public bool HasValue
		{
			get => hasValue;
			set => Set(ref hasValue, value);
		}

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

		public EditedSongProperty(T initialValue)
		{
			Value = initialValue;
		}

		public EditedSongProperty()
		{
			HasValue = false;
		}

		public override string ToString()
		{
			if (!HasValue)
			{
				return "< keep >";
			}

			return Value == null ? "< blank >" : Value.ToString();
		}

		public override bool Equals(Object obj)
		{
			EditedSongProperty<T> cmp = obj as EditedSongProperty<T>;
			if (cmp == null)
			{
				return false;
			}

			if (HasValue != cmp.HasValue)
			{
				return false;
			}

			if (!HasValue)
			{
				return true;
			}

			if (Value == null && cmp.Value == null)
			{
				return true;
			}

			if ((Value != null && cmp.Value == null) || (Value == null && cmp.Value != null))
			{
				return false;
			}

			return Value.Equals(cmp.Value);
		}

		public override int GetHashCode()
		{
			// Overflow is fine, just wrap
			unchecked
			{
				int hash = 17;
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
