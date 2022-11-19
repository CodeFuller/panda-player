using System;
using System.Collections.Generic;
using System.Linq;

namespace PandaPlayer.DiscAdder.ParsingContent
{
	internal class ReferenceContentParser : IReferenceContentParser
	{
		private readonly IInputContentSplitter inputContentSplitter;
		private readonly IReferenceDiscContentParser referenceDiscContentParser;

		public ReferenceContentParser(IInputContentSplitter inputContentSplitter, IReferenceDiscContentParser referenceDiscContentParser)
		{
			this.inputContentSplitter = inputContentSplitter ?? throw new ArgumentNullException(nameof(inputContentSplitter));
			this.referenceDiscContentParser = referenceDiscContentParser ?? throw new ArgumentNullException(nameof(referenceDiscContentParser));
		}

		public IEnumerable<ReferenceDiscContent> Parse(string rawReferenceContent)
		{
			return Parse(rawReferenceContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None));
		}

		private IEnumerable<ReferenceDiscContent> Parse(IEnumerable<string> content)
		{
			return inputContentSplitter.Split(content).
				Select(c => referenceDiscContentParser.Parse(c));
		}
	}
}
