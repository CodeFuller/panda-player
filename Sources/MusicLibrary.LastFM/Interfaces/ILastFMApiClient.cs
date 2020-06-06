using System.Threading.Tasks;
using MusicLibrary.LastFM.DataContracts;
using MusicLibrary.LastFM.Objects;

namespace MusicLibrary.LastFM.Interfaces
{
	public interface ILastFMApiClient
	{
		Task UpdateNowPlaying(Track track);

		Task Scrobble(TrackScrobble trackScrobble);

		Task<GetArtistInfoResponse> GetArtistInfo(string artistName, string userName);

		Task<GetAlbumInfoResponse> GetAlbumInfo(Album album, string userName);

		Task<GetTrackInfoResponse> GetTrackInfo(Track track, string userName);
	}
}
