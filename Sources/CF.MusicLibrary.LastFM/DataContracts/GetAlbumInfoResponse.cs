using System.Runtime.Serialization;

namespace CF.MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class GetAlbumInfoResponse
	{
		[DataMember]
		public AlbumInfo Album { get; set; }
	}
}
