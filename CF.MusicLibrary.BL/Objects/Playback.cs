using System;
using System.Runtime.Serialization;

namespace CF.MusicLibrary.BL.Objects
{
	/// <summary>
	/// Data contract for song playback occurrence.
	/// </summary>
	[DataContract]
	public class Playback
	{
		/// <summary>
		/// The time when playback has happened.
		/// </summary>
		[DataMember]
		public DateTime PlaybackTime { get; set; }
	}
}
