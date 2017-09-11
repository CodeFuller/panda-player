﻿using System;

namespace CF.MusicLibrary.LastFM.Objects
{
	public class Album
	{
		public string Artist { get; }

		public string Title { get; }

		public Album(string artistName, string albumTitle)
		{
			Artist = artistName;
			Title = albumTitle;
		}

		public override bool Equals(Object obj)
		{
			Album cmp = obj as Album;
			if (cmp == null)
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