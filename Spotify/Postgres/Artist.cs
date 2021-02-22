using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spotify
{
    public class Artist
	{
		public int artist_id { get; set; }
		public DateTime? created_at { get; set; }
		public DateTime? modified_at { get; set; }
		public string id { get; set; }
		public string name { get; set; }
		public int popularity { get; set; }
		public string type { get; set; }
		public string uri { get; set; }
		public int followers { get; set; }
		public string spotify_url { get; set; }
		public List<ArtistGenre> genres { get; set; }
		public List<ArtistImage> images { get; set; }
		public List<RelatedArtist> relatedArtists { get; set; }
		public List<Album> albums { get; set; }
		public List<ChartTrack> chartTracks { get; set; }

		public Artist()
        {
			genres = new List<ArtistGenre>();
			images = new List<ArtistImage>();
			relatedArtists = new List<RelatedArtist>();
			albums = new List<Album>();
			chartTracks = new List<ChartTrack>();
        }

		public List<string> GetAlbumIds()
		{
			return albums.Select(album => album.id).Distinct().ToList(); 
		}
		private List<string> GetAlbumTrackIds()
		{
			var trackIds = new List<string>();
			albums.ForEach(album => trackIds.AddRange(album.tracks.Select(track => track.id).ToList()));
			return trackIds.Distinct().ToList();
		}
		public void SetChartTracks()
        {

        }
		public void GetAlbums(SpotifyClient client)
        {
			try
			{
				var response = client.Artists.GetAlbums(id, new ArtistsAlbumsRequest() { IncludeGroupsParam = ArtistsAlbumsRequest.IncludeGroups.Album }).Result.Items;
				response.ForEach(a => 
				{
					var album = new Album()
					{
						id = a.Id,
						album_group = a.AlbumGroup,
						album_type = a.AlbumType,
						name = a.Name,
						release_date = a.ReleaseDate,
						release_date_precision = a.ReleaseDatePrecision,
						type = a.Type,
						uri = a.Uri,
						spotify_url = a.ExternalUrls.FirstOrDefault().Value
					};
                    if (albums.Contains(album)){						
						albums.Remove(album);
                    }
					albums.Add(album);
				});

				albums.FindAll(a => string.IsNullOrEmpty(a.album_group)).ForEach(a => a.album_group = a.album_type);
			}
			catch (Exception ex)
			{

			}
        }
		public void GetAlbumTracks(SpotifyClient client)
        {
			var trackIds = new List<string>();
			GetAlbumIds().ForEach(albumId => {
				var response = client.Albums.GetTracks(albumId).Result.Items;
				trackIds.AddRange(response.Select(track => track.Id).ToList());
			});

			trackIds = trackIds.Distinct().ToList();
			for (int i = 0; i < trackIds.Count; i += (trackIds.Count - i) >= 50 ? 50 : (trackIds.Count - i))
			{
                try
                {


					var request = new TracksRequest(trackIds.GetRange(i, ((trackIds.Count - i) >= 50 ? 50 : (trackIds.Count - i))));
					var response = client.Tracks.GetSeveral(request).Result.Tracks;
					response.ForEach(track =>
					{
						albums.Where(album => album.id == track.Album.Id).ToList().ForEach(album => {
							album.tracks.Add(new Track()
							{
								duration_ms = track.DurationMs,
								isexplicit = track.Explicit,
								name = track.Name,
								popularity = track.Popularity,
								preview_url = track.PreviewUrl ?? track.ExternalUrls.Values.FirstOrDefault(),
								track_number = track.TrackNumber,
								type = track.Type.ToString(),
								uri = track.Uri,
								spotify_url = track.ExternalUrls.Values.FirstOrDefault(),
								id = track.Id,
								disc_number = track.DiscNumber
							});
						});
					});                
				}
				catch(Exception ex)
                {

                }
			};
		}
		public void GetAlbumTracksAudioFeatures(SpotifyClient client)
		{
			var trackIds = GetAlbumTrackIds();
			for (int i = 0; i < trackIds.Count; i += (trackIds.Count - i) >= 50 ? 50 : (trackIds.Count - i))
			{
				var request = new TracksAudioFeaturesRequest(trackIds.GetRange(i, (trackIds.Count - i) >= 50 ? 50 : (trackIds.Count - i)));
				var response = client.Tracks.GetSeveralAudioFeatures(request).Result.AudioFeatures;
				client.Tracks.GetSeveralAudioFeatures(request).Result.AudioFeatures.ForEach(f =>
				{
					var trackFeature = new TrackFeatures()
					{
						danceability = (decimal)f.Danceability,
						energy = (decimal)f.Energy,
						key = f.Key,
						loudness = (decimal)f.Loudness,
						mode = f.Mode,
						speechiness = (decimal)f.Speechiness,
						acousticness = (decimal)f.Acousticness,
						instrumentalness = (decimal)f.Instrumentalness,
						liveness = (decimal)f.Liveness,
						valence = (decimal)f.Valence,
						tempo = (decimal)f.Tempo,
						duration_ms = f.DurationMs,
						time_signature = f.TimeSignature
					};
					albums.Where(album => album.HasTrack(f.Id)).ToList().ForEach(album => album.UpdateTrackFeature(f));
				});
			}
		}

		public void GetAlbumTracksAudioAnalysis(SpotifyClient client)
        {
			var audioanalysis = new Dictionary<string, TrackAudioAnalysis>();
			GetAlbumTrackIds().Distinct().ToList().ForEach(trackId => {
				var response = client.Tracks.GetAudioAnalysis(trackId).Result;
				audioanalysis.Add(trackId, response);
			});

			audioanalysis.ToList().ForEach(analysis => {
				albums.Where(album => album.HasTrack(analysis.Key)).ToList().ForEach(album => album.UpdateTrackAnalysis(analysis.Key, analysis.Value));
			});
        }
	}
}
