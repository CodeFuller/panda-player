﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.MusicLibrary.AlbumPreprocessor.Interfaces
{
	public interface ISongFileFilter
	{
		bool IsSongFile(string filename);
	}
}
