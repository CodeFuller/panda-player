﻿using System.Runtime.Serialization;

namespace MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class GetTrackInfoResponse
	{
		[DataMember]
		public TrackInfo Track { get; set; }
	}
}