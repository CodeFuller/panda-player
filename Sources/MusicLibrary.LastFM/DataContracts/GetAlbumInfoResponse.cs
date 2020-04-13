using System.Runtime.Serialization;

namespace MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class GetAlbumInfoResponse
	{
		[DataMember]
		public AlbumInfo Album { get; set; }
	}
}
