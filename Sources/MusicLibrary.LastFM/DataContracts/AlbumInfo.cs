using System.Runtime.Serialization;

namespace MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class AlbumInfo
	{
		[DataMember]
		public int UserPlayCount { get; set; }
	}
}
