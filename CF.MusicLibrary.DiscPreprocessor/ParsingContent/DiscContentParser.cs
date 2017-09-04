using System;
using System.Collections.Generic;
using System.Linq;

namespace CF.MusicLibrary.DiscPreprocessor.ParsingContent
{
	public class DiscContentParser : IDiscContentParser
	{
		private readonly IInputContentSplitter inputContentSplitter;
		private readonly IEthalonDiscParser ethalonDiscParser;

		public DiscContentParser(IInputContentSplitter inputContentSplitter, IEthalonDiscParser ethalonDiscParser)
		{
			if (inputContentSplitter == null)
			{
				throw new ArgumentNullException(nameof(inputContentSplitter));
			}
			if (ethalonDiscParser == null)
			{
				throw new ArgumentNullException(nameof(ethalonDiscParser));
			}

			this.inputContentSplitter = inputContentSplitter;
			this.ethalonDiscParser = ethalonDiscParser;
		}

		public IEnumerable<DiscContent> Parse(string content)
		{
			if (content == null)
			{
				throw new ArgumentNullException(nameof(content));
			}

			return Parse(content.Split(new [] { "\r\n", "\n" }, StringSplitOptions.None));
		}

		private IEnumerable<DiscContent> Parse(IEnumerable<string> content)
		{
			return inputContentSplitter.Split(content).
				Select(c => ethalonDiscParser.Parse(c));
		}
	}
}
