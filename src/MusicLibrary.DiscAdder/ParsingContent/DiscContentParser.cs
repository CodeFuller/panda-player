using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicLibrary.DiscAdder.ParsingContent
{
	internal class DiscContentParser : IDiscContentParser
	{
		private readonly IInputContentSplitter inputContentSplitter;
		private readonly IReferenceDiscParser referenceDiscParser;

		public DiscContentParser(IInputContentSplitter inputContentSplitter, IReferenceDiscParser referenceDiscParser)
		{
			this.inputContentSplitter = inputContentSplitter ?? throw new ArgumentNullException(nameof(inputContentSplitter));
			this.referenceDiscParser = referenceDiscParser ?? throw new ArgumentNullException(nameof(referenceDiscParser));
		}

		public IEnumerable<DiscContent> Parse(string content)
		{
			if (content == null)
			{
				throw new ArgumentNullException(nameof(content));
			}

			return Parse(content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None));
		}

		private IEnumerable<DiscContent> Parse(IEnumerable<string> content)
		{
			return inputContentSplitter.Split(content).
				Select(c => referenceDiscParser.Parse(c));
		}
	}
}
