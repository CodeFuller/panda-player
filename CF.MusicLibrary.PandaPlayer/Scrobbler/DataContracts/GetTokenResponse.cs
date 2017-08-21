using System.Runtime.Serialization;

namespace CF.MusicLibrary.PandaPlayer.Scrobbler.DataContracts
{
	[DataContract]
	public class GetTokenResponse
	{
		[DataMember]
		public string Token { get; set; }
	}
}
