using System.Runtime.Serialization;

namespace CF.MusicLibrary.LastFM.DataContracts
{
	[DataContract]
	public class GetTokenResponse
	{
		[DataMember]
		public string Token { get; set; }
	}
}
