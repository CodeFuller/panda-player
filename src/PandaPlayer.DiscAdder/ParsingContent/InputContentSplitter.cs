using System.Collections.Generic;
using System.Linq;

namespace PandaPlayer.DiscAdder.ParsingContent
{
	internal class InputContentSplitter : IInputContentSplitter
	{
		public IEnumerable<IReadOnlyCollection<string>> Split(IEnumerable<string> content)
		{
			var currentBlockContent = new List<string>();
			foreach (var line in content)
			{
				if (line.Length == 0)
				{
					if (currentBlockContent.Any())
					{
						yield return currentBlockContent;
						currentBlockContent = new List<string>();
					}
				}
				else
				{
					currentBlockContent.Add(line);
				}
			}

			if (currentBlockContent.Any())
			{
				yield return currentBlockContent;
			}
		}
	}
}
