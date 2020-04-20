﻿using System;
using MusicLibrary.Dal.Abstractions.Dto;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	public static class UriExtensions
	{
		public static ItemId ToItemId(this Uri uri)
		{
			return new ItemId(uri.OriginalString);
		}
	}
}