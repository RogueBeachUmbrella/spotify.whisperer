using System;
using System.Collections.Generic;
using SpotifyAPI.Web;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.IO;
using System.Collections;
using System.Net.Http.Headers;
using System.Text;
using System.Linq;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Configuration;
using Npgsql;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Spotify
{
    class Program
    {
        static void Main(string[] args)
        {
            var appSettings = ConfigurationManager.AppSettings;
            var connection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString);
            var spotifyClient = new SpotifyClient(appSettings["SpotifyToken"]);

            var chartTracks = GetChartTracks();
            var tracks = GetTracks(spotifyClient, chartTracks);
            //var trackAudioFeatures = GetTrackAudioFeatures(spotifyClient, chartTracks);
            //var trackAudioAnalysis = GetTrackAudioAnalysis(spotifyClient, chartTracks);


            var artists = GetTrackArtists(spotifyClient, tracks);
            //var relatedArtists = GetArtistRelatedArtists(spotifyClient, artists);
            //var artistAlbums = GetArtistAlbums(spotifyClient, artists);





            var pgTracks = PGLoadTracks(tracks);
            var pgArtists = PGLoadArtists(artists);
            var pgChartTracks = PGLoadChartTracks(chartTracks, pgArtists, pgTracks, tracks);


        }
        static List<Spotify.ChartTrack> GetChartTracks()
        {
            var url = @"https://spotifycharts.com/regional/us/weekly/2021-01-22--2021-01-29/download";

            var client = new HttpClient();
            var responseStream = client.GetStreamAsync(url).Result;
            var chartTracks = new List<Spotify.ChartTrack>();
            using (var reader = new StreamReader(responseStream))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.Read();
                    csv.ReadHeader();

                    chartTracks = csv.GetRecords<Spotify.ChartTrack>().ToList();
                }
            }
            return chartTracks;
        }
        static List<FullTrack> GetTracks(SpotifyClient spotifyClient, List<Spotify.ChartTrack> chartTracks)
        {
            var ids = chartTracks.Select(ct => ct.id).ToList();
            var tracks = new List<FullTrack>();
            for (int i = 0; i <= ids.Count - 50; i += 50)
            {
                var request = new TracksRequest(ids.GetRange(i, ((ids.Count - i) >= 50 ? 50 : (ids.Count - i))));
                tracks.AddRange(spotifyClient.Tracks.GetSeveral(request).Result.Tracks);
            }
            return tracks;
        }
        static Dictionary<string, TrackAudioFeatures> GetTrackAudioFeatures(SpotifyClient spotifyClient, List<Spotify.ChartTrack> chartTracks)
        {
            var ids = chartTracks.Select(ct => ct.id).ToList();
            var tracks = new Dictionary<string, TrackAudioFeatures>();
            for (int i = 0; i <= ids.Count - 50; i += 50)
            {
                var request = new TracksAudioFeaturesRequest(ids.GetRange(i, (ids.Count - i) >= 50 ? 50 : (ids.Count - i)));
                spotifyClient.Tracks.GetSeveralAudioFeatures(request).Result.AudioFeatures.ForEach(t =>
                {
                    tracks.Add(t.Id, t);
                });
            }
            return tracks;
        }
        static Dictionary<string, TrackAudioAnalysis> GetTrackAudioAnalysis(SpotifyClient spotifyClient, List<Spotify.ChartTrack> chartTracks)
        {
            var ids = chartTracks.Select(ct => ct.id).ToList();
            var tracks = new Dictionary<string, TrackAudioAnalysis>();
            chartTracks.ForEach(track => {
                var result = spotifyClient.Tracks.GetAudioAnalysis(track.id).Result;
                tracks.Add(track.id, result);            
            });
            return tracks;
        }
        static Dictionary<string, FullArtist> GetTrackArtists(SpotifyClient spotifyClient, List<FullTrack> fullTracks)
        {
            var ids = new List<string>();
            fullTracks.ForEach(ft => ids.AddRange(ft.Artists.Select(a => a.Id).ToList()));
            var artists = new Dictionary<string, FullArtist>();
            for (int i = 0; i <= ids.Count; i += 50)
            {
                var response = spotifyClient.Artists.GetSeveral(new ArtistsRequest(ids.GetRange(i, ((ids.Count - i) < 50 ? (ids.Count - i) : 50)))).Result.Artists;
                response.ForEach(a => {
                    artists.TryAdd(a.Id, a);
                });
            }
            return artists;
        }
        static Dictionary<string, List<SimpleAlbum>> GetArtistAlbums(SpotifyClient spotifyClient, Dictionary<string, FullArtist> artists)
        {
            var albums = new Dictionary<string, List<SimpleAlbum>>();
            foreach (var artist in artists)
            {
                albums.Add(artist.Key, spotifyClient.Artists.GetAlbums(artist.Key).Result.Items);
            }
            return albums;
        }
        static Dictionary<string, List<FullArtist>> GetArtistRelatedArtists(SpotifyClient spotifyClient, Dictionary<string, FullArtist> artists)
        {
            var relatedArtists = new Dictionary<string, List<FullArtist>>();
            foreach (var artist in artists)
            {
                relatedArtists.Add(artist.Key, spotifyClient.Artists.GetRelatedArtists(artist.Key).Result.Artists);
            }
            return relatedArtists;
        }
        static List<PgSQL.Track> PGLoadTracks(List<FullTrack> tracks)
        {
            var pgChartTracks = new List<PgSQL.Track>();
            string sqlQuery = "INSERT INTO track(duration_ms, explicit, name, popularity, preview_url, track_number, type, uri, spotify_url, id) VALUES (@duration_ms, @isexplicit, @name, @popularity, @preview_url, @track_number, @type, @uri, @spotify_url, @id) RETURNING track_id;";
            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                tracks.ForEach(track =>
                {
                    var pgTrack = new PgSQL.Track()
                    {
                        duration_ms = track.DurationMs,
                        isexplicit = track.Explicit,
                        name = track.Name,
                        popularity = track.Popularity,
                        preview_url = track.PreviewUrl,
                        track_number = track.TrackNumber,
                        type = track.Type.ToString(),
                        uri = track.Uri,
                        spotify_url = track.ExternalUrls.FirstOrDefault().Value,
                        id = track.Id
                    };
                    pgTrack.track_id = db.QuerySingle<int>(sqlQuery, pgTrack);
                    pgChartTracks.Add(pgTrack);
                });

                return pgChartTracks;
            }
        }
        static List<PgSQL.Artist> PGLoadArtists(Dictionary<string, FullArtist> artists)
        {

            var pgArtists = new List<PgSQL.Artist>();
            string sqlQuery = "INSERT INTO artist(id, name, popularity, type, uri, followers, spotify_url) VALUES (@id, @name, @popularity, @type, @uri, @followers, @spotify_url) returning artist_id;";  
            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                artists.ToList().ForEach(artist =>
                {
                    var pgArtist = new PgSQL.Artist()
                    {
                        id = artist.Value.Id,
                        name = artist.Value.Name,
                        popularity = artist.Value.Popularity,
                        type = artist.Value.Type,
                        uri = artist.Value.Uri,
                        followers = artist.Value.Followers.Total,
                        spotify_url = artist.Value.ExternalUrls.FirstOrDefault().Value
                    };
                    pgArtist.artist_id = db.QuerySingle<int>(sqlQuery, pgArtist);
                    pgArtists.Add(pgArtist);
                });
                return pgArtists;
            }
        }            
        static List<PgSQL.ChartTrack> PGLoadChartTracks(List<Spotify.ChartTrack> chartTracks, List<PgSQL.Artist> artists, List<PgSQL.Track> tracks, List<FullTrack> fullTracks)
        {
            var query = from chart in chartTracks
                        join track in tracks on chart.id equals track.id
                        join fullTrack in fullTracks on chart.id equals fullTrack.Id
                        join artist in artists on fullTrack.Artists.Where(a => a.Name == chart.artist).FirstOrDefault().Name equals artist.name
                        select new PgSQL.ChartTrack()
                        {
                            artist_id = artist.artist_id,
                            track_id = track.track_id,
                            position = chart.position,
                            streams = chart.streams,
                            country = "US",
                            week_start = new DateTime(2020, 1, 22),
                            week_end = new DateTime(2020, 1, 29)
                        };

            var pgChartTracks = new List<PgSQL.ChartTrack>();
            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {                
                string sqlQuery = "INSERT INTO chart_track(artist_id, track_id, \"position\", streams, country, week_start, week_end) VALUES(@artist_id, @track_id, @position, @streams, @country, @week_start, @week_end); ";
                query.ToList().ForEach(q => { 
                    db.Execute(sqlQuery, q);
                });
            }
            return pgChartTracks;
        }

    }
}
