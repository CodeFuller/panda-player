﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CF.MusicLibrary.PandaPlayer
{
	public class PandaPlayerSettings
	{
		public ICollection<string> DiscCoverImageLookupPages { get; } = new Collection<string>();
	}
}