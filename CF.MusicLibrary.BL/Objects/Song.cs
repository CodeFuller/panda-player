using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace CF.MusicLibrary.BL.Objects
{
	/// <summary>
	/// Data contract for song entity.
	/// </summary>
	[DataContract]
	public class Song
	{
		/// <summary>
		/// Song id.
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Song artist.
		/// </summary>
		[DataMember]
		public Artist Artist { get; set; }

		/// <summary>
		/// Song order number on the disc.
		/// </summary>
		[DataMember]
		public short OrderNumber { get; set; }

		/// <summary>
		/// Song year.
		/// </summary>
		[DataMember]
		public short? Year { get; set; }

		/// <summary>
		/// Song title.
		/// </summary>
		[DataMember]
		public string Title { get; set; }

		/// <summary>
		/// Song genre.
		/// </summary>
		[DataMember]
		public Genre Genre { get; set; }

		/// <summary>
		/// Song duration.
		/// </summary>
		[DataMember]
		public TimeSpan Duration { get; set; }

		/// <summary>
		/// Song rating.
		/// </summary>
		[DataMember]
		public Rating? Rating { get; set; }

		/// <summary>
		/// Song file path.
		/// </summary>
		[DataMember]
		public Uri Uri { get; set; }

		/// <summary>
		/// Song file size.
		/// </summary>
		[DataMember]
		public int FileSize { get; set; }

		/// <summary>
		/// Song bitrate.
		/// </summary>
		[DataMember]
		public int? Bitrate { get; set; }

		/// <summary>
		/// Song last playback time.
		/// </summary>
		[DataMember]
		public DateTime? LastPlaybackTime { get; set; }

		/// <summary>
		/// Song playbacks count.
		/// </summary>
		[DataMember]
		public int PlaybacksCount { get; set; }

		/// <summary>
		/// Song playback occurrences.
		/// </summary>
		[DataMember]
		public Collection<Playback> Playbacks { get; } = new Collection<Playback>();
	}
}
