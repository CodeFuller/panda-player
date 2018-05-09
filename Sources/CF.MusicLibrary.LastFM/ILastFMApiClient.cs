using System.Threading.Tasks;
using CF.MusicLibrary.LastFM.DataContracts;
using CF.MusicLibrary.LastFM.Objects;

namespace CF.MusicLibrary.LastFM
{
	public interface ILastFMApiClient
	{
		Task OpenSession();

		Task UpdateNowPlaying(Track track);

		Task Scrobble(TrackScrobble trackScrobble);

		Task<GetArtistInfoResponse> GetArtistInfo(string artistName, string userName);

		Task<GetAlbumInfoResponse> GetAlbumInfo(Album album, string userName);

		Task<GetTrackInfoResponse> GetTrackInfo(Track track, string userName);
	}
}
