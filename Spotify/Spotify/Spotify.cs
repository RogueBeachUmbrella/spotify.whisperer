using CsvHelper;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Spotify
{
    public class Spotify
    {
        private SpotifyClient _client { get; set; }
        public Spotify()
        {
            _client = new SpotifyClient(ConfigurationManager.AppSettings["SpotifyToken"]);
        }
        public List<ChartReportTrack> GetChartTracks()
        {
            var url = @"https://spotifycharts.com/regional/us/weekly/2021-02-05--2021-02-12/download";

            var client = new HttpClient();
            var responseStream = client.GetStreamAsync(url).Result;
            var chartTracks = new List<ChartReportTrack>();
            using (var reader = new StreamReader(responseStream))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.Read();
                    csv.ReadHeader();

                    chartTracks = csv.GetRecords<ChartReportTrack>().ToList();
                }
            }
            return chartTracks;
        }
        public List<FullTrack> GetTracks(List<string> ids)
        {
            var tracks = new List<FullTrack>();
            for (int i = 0; i <= ids.Count - 50; i += 50)
            {
                var request = new TracksRequest(ids.GetRange(i, ((ids.Count - i) >= 50 ? 50 : (ids.Count - i))));
                tracks.AddRange(_client.Tracks.GetSeveral(request).Result.Tracks);
            }
            return tracks;
        }
        public Dictionary<string, TrackAudioFeatures> GetTrackAudioFeatures(List<ChartReportTrack> chartTracks)
        {
            var ids = chartTracks.Select(ct => ct.id).ToList();
            var tracks = new Dictionary<string, TrackAudioFeatures>();
            for (int i = 0; i <= ids.Count - 50; i += 50)
            {
                var request = new TracksAudioFeaturesRequest(ids.GetRange(i, (ids.Count - i) >= 50 ? 50 : (ids.Count - i)));
                _client.Tracks.GetSeveralAudioFeatures(request).Result.AudioFeatures.ForEach(t =>
                {
                    tracks.Add(t.Id, t);
                });
            }
            return tracks;
        }
        public Dictionary<string, SpotifyAPI.Web.TrackAudioAnalysis> GetTrackAudioAnalysis(List<ChartReportTrack> chartTracks)
        {
            var ids = chartTracks.Select(ct => ct.id).ToList();
            var tracks = new Dictionary<string, SpotifyAPI.Web.TrackAudioAnalysis>();
            chartTracks.ForEach(track => {
                var result = _client.Tracks.GetAudioAnalysis(track.id).Result;
                tracks.Add(track.id, result);
            });
            return tracks;
        }
        public Dictionary<string, FullArtist> GetTrackArtists(List<FullTrack> fullTracks)
        {
            var ids = new List<string>();
            fullTracks.ForEach(ft => ids.AddRange(ft.Artists.Select(a => a.Id).ToList()));
            var artists = new Dictionary<string, FullArtist>();
            for (int i = 0; i <= ids.Count; i += 50)
            {
                var response = _client.Artists.GetSeveral(new ArtistsRequest(ids.GetRange(i, ((ids.Count - i) < 50 ? (ids.Count - i) : 50)))).Result.Artists;
                response.ForEach(a => {
                    artists.TryAdd(a.Id, a);
                });
            }
            return artists;
        }
        public Dictionary<string, List<SimpleAlbum>> GetArtistAlbums(Dictionary<string, FullArtist> artists)
        {
            var albums = new Dictionary<string, List<SimpleAlbum>>();
            foreach (var artist in artists)
            {
                try
                {
                    var response = _client.Artists.GetAlbums(artist.Key).Result.Items;
                    albums.TryAdd(artist.Key, response);
                }
                catch(Exception ex)
                {

                }

            }
            return albums;
        }
        public Dictionary<string, List<FullArtist>> GetArtistRelatedArtists(Dictionary<string, FullArtist> artists)
        {
            var relatedArtists = new Dictionary<string, List<FullArtist>>();
            foreach (var artist in artists)
            {
                try { 
                    var response = _client.Artists.GetRelatedArtists(artist.Key).Result.Artists;
                    relatedArtists.TryAdd(artist.Key, response);                
                }
                catch(Exception ex)
                {

                }
            }
            return relatedArtists;
        }
        public Dictionary<string, List<SimpleTrack>> GetAlbumTracks(Dictionary<string, List<SimpleAlbum>> albums)
        {
            var albumTracks = new Dictionary<string, List<SimpleTrack>>();
            try
            {
                albums.Values.ToList().ForEach(x => x.ForEach(a => {
                    var response = _client.Albums.GetTracks(a.Id).Result.Items.ToList();
                    albumTracks.TryAdd(a.Id, response);
                }));
            }
            catch(Exception ex)
            {

            }
            return albumTracks;
        }
        public void JoinArtistData(Dictionary<string, FullArtist> artists,
                             Dictionary<string, List<FullArtist>> relatedArtists,
                             Dictionary<string, List<SimpleAlbum>> artistAlbums,
                             Dictionary<string, List<SimpleTrack>> albumTracks)
        {


            var query = from a in artists
                        join ra in relatedArtists on a.Key equals ra.Key
                        join aa in artistAlbums on a.Key equals aa.Key
                        select new Artist
                        {
                            id = a.Key,
                            name = a.Value.Name,
                            popularity = a.Value.Popularity,
                            type = a.Value.Type,
                            uri = a.Value.Uri,
                            followers = a.Value.Followers.Total,
                            spotify_url = a.Value.ExternalUrls.FirstOrDefault().Value,
                            relatedArtists = ra.Value.Select(ra => new RelatedArtist() { Id = ra.Id}).ToList(),
                            albums = (from album in aa.Value
                                     join tracks in albumTracks on album.Id equals tracks.Key
                                     select new Album
                                     {
                                         id = album.Id,
                                         album_group = album.AlbumGroup,
                                         album_type = album.AlbumType,
                                         name = album.Name,
                                         release_date = album.ReleaseDate,
                                         release_date_precision = album.ReleaseDatePrecision,
                                         total_tracks = tracks.Value.Count,
                                         type = album.Type,
                                         uri = album.Uri,
                                         tracks = tracks.Value.Select(t => new Track() {
                                             duration_ms = t.DurationMs,
                                             isexplicit = t.Explicit,
                                             name = t.Name,
                                             preview_url = t.PreviewUrl,
                                             track_number = t.TrackNumber,
                                             //type = t.Type
                                             uri = t.Uri,
                                             spotify_url = t.ExternalUrls.Values.FirstOrDefault(),
                                             id = t.Id
                                         }).ToList()

                                     }).ToList()
                        };

            query.ToList();
            

        }
    }  
}
