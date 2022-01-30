using System;

namespace PandaPlayer.Core.Models
{
	public class ItemId
	{
		public string Value { get; }

		public ItemId(string id)
		{
			Value = id ?? throw new ArgumentNullException(nameof(id));
		}

		public static bool operator ==(ItemId v1, ItemId v2)
		{
			if (ReferenceEquals(v1, null) || ReferenceEquals(v2, null))
			{
				return ReferenceEquals(v1, null) && ReferenceEquals(v2, null);
			}

			return v1.Equals(v2);
		}

		public static bool operator !=(ItemId v1, ItemId v2)
		{
			return !(v1 == v2);
		}

		public override bool Equals(object obj)
		{
			return obj is ItemId cmp && Equals(cmp);
		}

		protected bool Equals(ItemId other)
		{
			return String.Equals(Value, other.Value, StringComparison.Ordinal);
		}

		public override int GetHashCode()
		{
			return Value?.GetHashCode(StringComparison.Ordinal) ?? 0;
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
