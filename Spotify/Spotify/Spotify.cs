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
            return chartTracks.GetRange(0, 4);
        }
        public (List<Track>, List<Artist>, List<ChartReportTrack>) GetChartData(List<ChartReportTrack> chartReportTracks)
        {
            var trackIds = chartReportTracks.Select(r => r.spotifyTrackId).ToList();
            var artistIds = new List<string>();
            var tracks = new List<Track>();
            var artists = new List<Artist>();
            for (int i = 0; i < trackIds.Count; i += (trackIds.Count - i) >= 50 ? 50 : (trackIds.Count - i))
            {
                var request = new TracksRequest(trackIds.GetRange(i, ((trackIds.Count - i) >= 50 ? 50 : (trackIds.Count - i))));
                var tracksResponse = _client.Tracks.GetSeveral(request).Result.Tracks;
                tracksResponse.ForEach(t => 
                {
                    var track = new Track()
                    {
                        duration_ms = t.DurationMs,
                        isexplicit = t.Explicit,
                        name = t.Name,
                        popularity = t.Popularity,
                        preview_url = t.PreviewUrl,
                        track_number = t.TrackNumber,
                        type = t.Type.ToString(),
                        uri = t.Uri,
                        spotify_url = t.ExternalUrls.Values.FirstOrDefault(),
                        id = t.Id,
                    };
                    tracks.Add(track);
                    artistIds.AddRange(t.Artists.Select(a => a.Id).ToList());
                    chartReportTracks.Where(c => c.spotifyTrackId == t.Id).FirstOrDefault().spotifyArtistIds.AddRange(t.Artists.Select(a => a.Id).ToList());
                });

                
            }

            for (int i = 0; i < artistIds.Count; i += (artistIds.Count - i) >= 50 ? 50 : (artistIds.Count - i))
            {
                var artistsResponse = _client.Artists.GetSeveral(new ArtistsRequest(artistIds.GetRange(i, ((artistIds.Count - i) < 50 ? (artistIds.Count - i) : 50)))).Result.Artists;
                artistsResponse.ForEach(a => {
                    var artist = new Artist()
                    {
                        id = a.Id,
                        name = a.Name,
                        type = a.Type,
                        popularity = a.Popularity,
                        followers = a.Followers.Total,
                        uri = a.Uri,
                        spotify_url = a.ExternalUrls.Values.FirstOrDefault()
                    };

                    a.Images.ForEach(img => {
                        var image = new ArtistImage()
                        {
                            url = img.Url,
                            height = img.Height,
                            width = img.Width
                        };
                        artist.images.Add(image);
                    });

                    a.Genres.ForEach(g => {
                        artist.genres.Add(new ArtistGenre() { genre = g });
                    });
                    artists.Add(artist);
                });
            }
            return (tracks, artists, chartReportTracks);
        }
        public (List<Artist>, List<Track>) GetArtistAlbumTracks(List<Artist> artists)
        {
            var albumTracks = new List<Track>();
            var ids = new List<string>();
            artists.ForEach(artist => {
                artist.albums.ForEach(album =>
                {
                    ids.AddRange(album.tracks.Select(track => track.id).ToList());
                });
            });

            for (int i = 0; i < ids.Count; i += (ids.Count - i) >= 50 ? 50 : (ids.Count - i))
            {
                var request = new TracksRequest(ids.GetRange(i, ((ids.Count - i) >= 50 ? 50 : (ids.Count - i))));
                var response = _client.Tracks.GetSeveral(request).Result.Tracks;
                response.ForEach(t =>
                {
                    var track = new Track()
                    {
                        duration_ms = t.DurationMs,
                        isexplicit = t.Explicit,
                        name = t.Name,
                        popularity = t.Popularity,
                        preview_url = t.PreviewUrl,
                        track_number = t.TrackNumber,
                        type = t.Type.ToString(),
                        uri = t.Uri,
                        spotify_url = t.ExternalUrls.Values.FirstOrDefault(),
                        id = t.Id,
                        spotifyAlbumId = t.Album.Id
                    };
                    albumTracks.Add(track);                   
                });

                artists.ForEach(artist => {
                    artist.albums.ForEach(album =>
                    {
                        album.tracks.AddRange(albumTracks.Where(track => track.spotifyAlbumId == album.id));
                    });
                });
            };
            return (artists, albumTracks);
        }
        public List<ChartReportTrack> UpdateChartReportTrackArtistId(List<Track> tracks, List<ChartReportTrack> chartReportTracks)
        {
            var query = from chart in chartReportTracks
                        join track in tracks on chart.spotifyTrackId equals track.id
                        select new ChartReportTrack()
                        {
                            position = chart.position,
                            trackName = chart.trackName,
                            artist = chart.artist,
                            streams = chart.streams,
                            url = chart.url,
                        };
            return query.ToList();
        }
        public List<Track> GetTrackAudioFeatures(List<Track> tracks)
        {
            //var trackFeatures = new TrackFeatures();
            var ids = tracks.Select(ct => ct.id).ToList();
            for (int i = 0; i < ids.Count; i += (ids.Count - i) >= 50 ? 50 : (ids.Count - i))
            {
                var request = new TracksAudioFeaturesRequest(ids.GetRange(i, (ids.Count - i) >= 50 ? 50 : (ids.Count - i)));
                _client.Tracks.GetSeveralAudioFeatures(request).Result.AudioFeatures.ForEach(f =>
                {
                    var trackFeatures = new TrackFeatures()
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
                    tracks.Where(t => t.id == f.Id).FirstOrDefault().features = trackFeatures;
                });

            }
            return tracks;
        }
        public List<Track> GetTrackAudioAnalysis(List<Track> tracks)
        {
            var ids = tracks.Select(t => t.id).ToList();
            tracks.ForEach(track => {
                var result = _client.Tracks.GetAudioAnalysis(track.id).Result;
               
                var trackAudioAnalysis = new TrackAudioAnalysis()
                {
                    num_samples = result.Track.num_samples,
                    duration = (decimal)result.Track.duration,
                    sample_md5 = result.Track.sample_md5,
                    offset_seconds = result.Track.offset_seconds,
                    window_seconds = result.Track.window_seconds,
                    analysis_sample_rate = result.Track.analysis_sample_rate,
                    analysis_channels = result.Track.analysis_channels,
                    end_of_fade_in = (decimal)result.Track.end_of_fade_in,
                    start_of_fade_out = (decimal)result.Track.start_of_fade_out,
                    loudness = (decimal)result.Track.loudness,
                    tempo = (decimal)result.Track.tempo,
                    tempo_confidence = (decimal)result.Track.tempo_confidence,
                    time_signature = result.Track.time_signature,
                    time_signature_confidence = (decimal)result.Track.time_signature_confidence,
                    key = result.Track.key,
                    key_confidence = (decimal)result.Track.key_confidence,
                    mode = result.Track.mode,
                    mode_confidence = (decimal)result.Track.mode_confidence
                };

                result.Sections.ForEach(s => {
                    var section = new TrackSection()
                    {
                        start = (decimal)s.Start,
                        duration = (decimal)s.Duration,
                        confidence = (decimal)s.Confidence,
                        loudness = (decimal)s.Loudness,
                        tempo = (decimal)s.Tempo,
                        tempo_confidence = (decimal)s.TempoConfidence,  
                        key = s.Key,
                        key_confidence = (decimal)s.KeyConfidence,
                        mode = s.Mode,
                        mode_confidence = (decimal)s.ModeConfidence,
                        time_signature = s.TimeSignature,
                        time_signature_confidence = (decimal)s.TimeSignatureConfidence
                    };
                    trackAudioAnalysis.sections.Add(section);
                });

                result.Segments.ForEach(s => {
                    var segment = new TrackSegment()
                    {
                        start = (decimal)s.Start,
                        duration = (decimal)s.Duration,
                        confidence = (decimal)s.Confidence,
                        loudness_start = (decimal)s.LoudnessStart,
                        loudness_max_time = (decimal)s.LoudnessMaxTime, 
                        loudness_max = (decimal)s.LoudnessMax,
                        loudness_end = (decimal)s.LoudnessEnd
                    };
                    s.Pitches.ForEach(pitch => { segment.pitches.Add(new Pitch() { pitch = (decimal)pitch }); });
                    s.Timbre.ForEach(timbre => { segment.timbres.Add(new Timbre() { timbre = (decimal)timbre }); });
                    trackAudioAnalysis.segments.Add(segment);
                });

                result.Tatums.ForEach(t => {
                    var tatum = new TrackTatum() { 
                        start = (decimal)t.Start,
                        duration = (decimal)t.Duration,
                        confidence = (decimal)t.Confidence
                    };
                    trackAudioAnalysis.tatums.Add(tatum);
                });

                track.audioAnalysis = trackAudioAnalysis;
            });
            return tracks;
        }
        public Dictionary<string, FullArtist> GetTrackArtists(List<FullTrack> fullTracks)
        {
            var ids = new List<string>();
            fullTracks.ForEach(ft => ids.AddRange(ft.Artists.Select(a => a.Id).ToList()));
            var artists = new Dictionary<string, FullArtist>();
            for (int i = 0; i < ids.Count; i += (ids.Count - i) >= 50 ? 50 : (ids.Count - i))
            {
                var response = _client.Artists.GetSeveral(new ArtistsRequest(ids.GetRange(i, ((ids.Count - i) < 50 ? (ids.Count - i) : 50)))).Result.Artists;
                response.ForEach(a => {
                    artists.TryAdd(a.Id, a);
                });
            }
            return artists;
        }
        public List<Artist> GetArtistAlbums(List<Artist> artists)
        {
            foreach (var artist in artists)
            {
                try
                {
                    var response = _client.Artists.GetAlbums(artist.id).Result.Items;
                    response.ForEach(a => {
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
                        artist.albums.Add(album);
                    });
                }
                catch(Exception ex)
                {

                }

            }
            return artists;
        }
        public List<Artist> GetArtistRelatedArtists(List<Artist> artists)
        {
            foreach (var artist in artists)
            {
                try { 
                    var response = _client.Artists.GetRelatedArtists(artist.id).Result.Artists;
                    response.ForEach(a => {
                        var relatedArtist = new RelatedArtist() { Id = a.Id };
                        artist.relatedArtists.Add(relatedArtist);
                    });              
                }
                catch(Exception ex)
                {

                }
            }
            return artists;
        }
        public List<Artist> GetAlbumTracks(List<Artist> artists)
        {
            try
            {
                artists.ForEach(artist => {
                    artist.albums.ForEach(album => { 
                        var response = _client.Albums.GetTracks(album.id).Result.Items.ToList();
                        response.ForEach(t => {
                            var track = new Track()
                            {
                                duration_ms = t.DurationMs,
                                isexplicit = t.Explicit,
                                name = t.Name,
                                preview_url = t.PreviewUrl,
                                track_number = t.TrackNumber,
                                type = t.Type.ToString(),
                                uri = t.Uri,
                                spotify_url = t.ExternalUrls.Values.FirstOrDefault(),
                                id = t.Id,
                            };
                            album.tracks.Add(track);
                        });
                    });
                });
            }
            catch(Exception ex)
            {

            }
            return artists;
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
