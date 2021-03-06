﻿using System.Collections.Generic;
using System.Linq;

namespace PandaPlayer.DiscAdder.ParsingContent
{
	internal class InputContentSplitter : IInputContentSplitter
	{
		public IEnumerable<IEnumerable<string>> Split(IEnumerable<string> content)
		{
			var currentBlockContent = new List<string>();
			foreach (var str in content)
			{
				if (str.Length == 0)
				{
					if (currentBlockContent.Any())
					{
						yield return currentBlockContent;
						currentBlockContent = new List<string>();
					}
				}
				else
				{
					currentBlockContent.Add(str);
				}
			}

			if (currentBlockContent.Any())
			{
				yield return currentBlockContent;
			}
		}
	}
}
