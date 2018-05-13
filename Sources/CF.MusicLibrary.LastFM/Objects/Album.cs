using System;

namespace CF.MusicLibrary.LastFM.Objects
{
	public class Album
	{
		public string Artist { get; set; }

		public string Title { get; set; }

		public override bool Equals(Object obj)
		{
			if (!(obj is Album cmp))
			{
				return false;
			}

			return Artist == cmp.Artist && Title == cmp.Title;
		}

		public override int GetHashCode()
		{
			// Overflow is fine, just wrap
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + Artist.GetHashCode();
				hash = hash * 23 + Title.GetHashCode();
				return hash;
			}
		}
	}
}
