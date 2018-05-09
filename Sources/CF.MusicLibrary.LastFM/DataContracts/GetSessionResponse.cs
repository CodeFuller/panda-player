using System.Runtime.Serialization;

namespace CF.MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class GetSessionResponse
	{
		[DataMember]
		public LastFMSession Session { get; set; }
	}
}
