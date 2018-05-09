using System.Runtime.Serialization;

namespace CF.MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class AlbumInfo
	{
		[DataMember]
		public int UserPlayCount { get; set; }
	}
}
