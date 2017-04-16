using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace CF.MusicLibrary.BL.Objects
{
	/// <summary>
	/// Data contract for the library Disc entity.
	/// </summary>
	[DataContract]
	public class Disc
	{
		/// <summary>
		/// Disc id.
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Disc title.
		/// </summary>
		[DataMember]
		public string Title { get; set; }

		/// <summary>
		/// Disc uri.
		/// </summary>
		[DataMember]
		public Uri Uri { get; set; }

		/// <summary>
		/// Disc songs.
		/// </summary>
		[DataMember]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is required by deserializer.")]
		public Collection<Song> Songs { get; set; } = new Collection<Song>();
	}
}
