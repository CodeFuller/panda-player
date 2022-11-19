using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PandaPlayer.Dal.LocalDb.Internal
{
	internal class FilePath : IEnumerable<string>
	{
		private readonly IReadOnlyCollection<string> path;

		public FilePath(IEnumerable<string> path)
		{
			this.path = path?.ToList() ?? throw new ArgumentNullException(nameof(path));
		}

		public FilePath Add(string part)
		{
			if (String.IsNullOrWhiteSpace(part))
			{
				throw new InvalidOperationException($"The path part is invalid: '{part}'");
			}

			return new FilePath(path.Concat(Enumerable.Repeat(part, 1)));
		}

		public IEnumerator<string> GetEnumerator()
		{
			return path.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
