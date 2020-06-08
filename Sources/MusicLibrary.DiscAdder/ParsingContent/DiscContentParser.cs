using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicLibrary.DiscPreprocessor.ParsingContent
{
	public class DiscContentParser : IDiscContentParser
	{
		private readonly IInputContentSplitter inputContentSplitter;
		private readonly IEthalonDiscParser ethalonDiscParser;

		public DiscContentParser(IInputContentSplitter inputContentSplitter, IEthalonDiscParser ethalonDiscParser)
		{
			this.inputContentSplitter = inputContentSplitter ?? throw new ArgumentNullException(nameof(inputContentSplitter));
			this.ethalonDiscParser = ethalonDiscParser ?? throw new ArgumentNullException(nameof(ethalonDiscParser));
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
				Select(c => ethalonDiscParser.Parse(c));
		}
	}
}
