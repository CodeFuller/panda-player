using System;
using Moq;

namespace PandaPlayer.UnitTests.Helpers
{
	// https://stackoverflow.com/a/70949014
	[TypeMatcher]
#pragma warning disable CA1067 // Override Object.Equals(object) when implementing IEquatable<T>
	public sealed class IsAnyToken : ITypeMatcher, IEquatable<IsAnyToken>
#pragma warning restore CA1067 // Override Object.Equals(object) when implementing IEquatable<T>
	{
		public bool Matches(Type typeArgument) => true;

#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
		public bool Equals(IsAnyToken other) => throw new NotImplementedException();
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
	}
}
