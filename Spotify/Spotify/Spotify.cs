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
using Spotify.SpotifyModels;

namespace Spotify
{
    public class Spotify
    {
        public SpotifyClient client { get; set; }
        private string token { get; set; }
        public Spotify()
        {
            client = new SpotifyClient(GetAccessToken());
        }
        public string GetAccessToken()
        {
            var clientId = ConfigurationManager.AppSettings["CLIENT_ID"];
            var clientSecret = ConfigurationManager.AppSettings["CLIENT_SECRET"];

            var config = SpotifyClientConfig.CreateDefault();
            var request = new ClientCredentialsRequest(clientId, clientSecret);
            return new OAuthClient(config).RequestToken(request).Result.AccessToken;
        }
        public void RefreshToken()
        {
            client = new SpotifyClient(GetAccessToken());
        }
        public List<ChartReportTrack> GetChartTracks(string countryCode, DateTime weekStart, DateTime weekEnd)
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
            //return chartTracks.ToList();
            return chartTracks.GetRange(1, 1);
        }
        public (Dictionary<string, SpotifyModels.Track> tracks, Dictionary<string, SpotifyModels.Artist> artists, Dictionary<string, SpotifyModels.Album> albums, List<ChartTrack> chartTracks) GetChartData(List<ChartReportTrack> chartReportTracks, string countryCode, DateTime weekStart, DateTime weekEnd)
        {
            var tracks = new Dictionary<string, SpotifyModels.Track>();
            var artists = new Dictionary<string, SpotifyModels.Artist>();
            var albums = new Dictionary<string, SpotifyModels.Album>();
            var chartTracks = new List<ChartTrack>();

            for (int i = 0; i < chartReportTracks.Count; i += (chartReportTracks.Count - i) >= 50 ? 50 : (chartReportTracks.Count - i))
            {
                var request = new TracksRequest(chartReportTracks.Select(x => x.id).ToList().GetRange(i, (chartReportTracks.Count - i) >= 50 ? 50 : (chartReportTracks.Count - i)));
                var tracksResponse = client.Tracks.GetSeveral(request).Result.Tracks;
                tracksResponse.ForEach(track => 
                {
                    tracks.TryAdd(track.Id,
                                  new SpotifyModels.Track() { 
                                      Id = track.Id, 
                                      //AlbumId = track.Album.Id, 
                                      //ArtistIds = track.Artists.Select(a => a.Id).ToList(), 
                                      DiscNumber = track.DiscNumber, 
                                      DurationMs = track.DurationMs, 
                                      Explicit = track.Explicit, 
                                      ExternalUrl = track.ExternalUrls.FirstOrDefault().Value, 
                                      Href = track.Href, 
                                      Name = track.Name, 
                                      Popularity = track.Popularity, 
                                      PreviewUrl = track.PreviewUrl, 
                                      TrackNumber = track.TrackNumber 
                                  });


                    track.Artists.ForEach(a => { 
                        artists.TryAdd(a.Id, new SpotifyModels.Artist() { 
                            Id = a.Id,
                            Name = a.Name,
                            Type = a.Type,
                            Uri = a.Uri,
                            ExternalUrl = a.ExternalUrls.Values.FirstOrDefault(),
                            Href = a.Href
                        });     
                        
                        albums.TryAdd(a.Id, new SpotifyModels.Album()
                        {
                            Id = track.Album.Id,
                            AlbumType = track.Album.AlbumType,
                            Name = track.Album.Name,
                            ReleaseDate = track.Album.ReleaseDate,
                            ReleaseDatePrecision = track.Album.ReleaseDatePrecision,
                            Type = track.Album.Type,
                            Uri = track.Album.Uri,
                            ExternalUrl = track.Album.ExternalUrls.FirstOrDefault().Value,
                            Href = track.Album.Href,
                            Images = track.Album.Images,
                            Artists = track.Album.Artists.Select(a => a.Id).ToList(),
                        });

                        var chartTrack = new ChartTrack()
                        {
                            position = chartReportTracks.Where(chartTrack => chartTrack.id == track.Id).Single().position,
                            streams = chartReportTracks.Where(chartTrack => chartTrack.id == track.Id).Single().streams,
                            country = countryCode,
                            week_start = weekStart.ToString("yyyy-MM-dd"),
                            week_end = weekEnd.ToString("yyyy-MM-dd")
                        };
                    });                   
                });             
            }

            return (tracks, artists, albums, chartTracks);
        }
        public List<FullArtist> GetSeveralArtistsFull(List<string> artistIds)
        {
            var artists = new List<FullArtist>();
            for (int i = 0; i < artistIds.Count; i += (artistIds.Count - i) >= 50 ? 50 : (artistIds.Count - i))
            {
                artists.AddRange(client.Artists.GetSeveral(new ArtistsRequest(artistIds.GetRange(i, ((artistIds.Count - i) < 50 ? (artistIds.Count - i) : 50)))).Result.Artists);
            }
            return artists;
        }
        public List<SimpleAlbum> GetArtistAlbums(List<string> artistIds) //TODO: Call album endpoint?
        {          
            var albums = new List<SimpleAlbum>();
            try
            {
                artistIds.ForEach(id => { 
                    var response = client.Artists.GetAlbums(id, new ArtistsAlbumsRequest() { IncludeGroupsParam = ArtistsAlbumsRequest.IncludeGroups.Album }).Result.Items;
                    
                    
                    response.ForEach(a => {
                        //var album = new Album()
                        //{
                        //    id = a.Id,
                        //    album_group = a.AlbumGroup,
                        //    album_type = a.AlbumType,
                        //    name = a.Name,
                        //    release_date = a.ReleaseDate,
                        //    release_date_precision = a.ReleaseDatePrecision,
                        //    type = a.Type,
                        //    uri = a.Uri,
                        //    spotify_url = a.ExternalUrls.FirstOrDefault().Value
                        //};
                        albums.Add(a);
                    });         
                });
            }
            catch(Exception ex)
            {

            }
            return albums;
        }            
        public List<FullAlbum> GetFullAlbums(List<string> ids)
        {
            var fullAlbums = new List<FullAlbum>();
            for (int i = 0; i < ids.Count; i += (ids.Count - i) >= 20 ? 20 : (ids.Count - i))
            {
                var response = client.Albums.GetSeveral(new AlbumsRequest(ids.GetRange(i, (ids.Count - i) < 20 ? (ids.Count - i) : 20))).Result;
                fullAlbums.AddRange(response.Albums);
            }
            return fullAlbums;
        }

        public void GetArtistAlbumTracks(ref Artist artist)
        {
            var ids = new List<string>();
            artist.albums.ForEach(album => {
                var response = client.Albums.GetTracks(album.id).Result.Items.Select(t => t.Id).ToList();
                ids.AddRange(response);
            });

            var tracks = new List<KeyValuePair<string, Track>>();
            for (int i = 0; i < ids.Count; i += (ids.Count - i) >= 50 ? 50 : (ids.Count - i))
            {
                var request = new TracksRequest(ids.GetRange(i, ((ids.Count - i) >= 50 ? 50 : (ids.Count - i))));
                var response = client.Tracks.GetSeveral(request).Result.Tracks;
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
                        disc_number = t.DiscNumber
                    };
                    tracks.Add(new KeyValuePair<string, Track>(t.Album.Id, track));
                });
            };
            var albums = new List<Album>();
            artist.albums.ForEach(a =>
            {
                a.tracks.AddRange(tracks.Where(t => t.Key == a.id).Select(t => t.Value).ToList());
                albums.Add(a);
            });
            artist.albums = albums;
        }
        public void GetTrackAudioAnalysis(ref Artist artist)
        {
            var trackAnalysis = new Dictionary<string, AudioAnalysis>();
            var album = new Album();
            var albums = new List<Album>();
            var track = new Track();
            var tracks = new List<Track>();
            artist.albums.ForEach(a => {
                album = a;
                a.tracks.ForEach(t =>
                {
                    track = t;
                    if (trackAnalysis.TryGetValue(t.id, out AudioAnalysis analysis))
                    {
                        t.audioAnalysis = analysis;
                    }
                    else
                    {
                        var result = client.Tracks.GetAudioAnalysis(t.id).Result;
                        track.audioAnalysis = new AudioAnalysis()
                        {
                            num_samples = result.Analysis.num_samples,
                            duration = (decimal)result.Analysis.duration,
                            sample_md5 = result.Analysis.sample_md5,
                            offset_seconds = result.Analysis.offset_seconds,
                            window_seconds = result.Analysis.window_seconds,
                            analysis_sample_rate = result.Analysis.analysis_sample_rate,
                            analysis_channels = result.Analysis.analysis_channels,
                            end_of_fade_in = (decimal)result.Analysis.end_of_fade_in,
                            start_of_fade_out = (decimal)result.Analysis.start_of_fade_out,
                            loudness = (decimal)result.Analysis.loudness,
                            tempo = (decimal)result.Analysis.tempo,
                            tempo_confidence = (decimal)result.Analysis.tempo_confidence,
                            time_signature = result.Analysis.time_signature,
                            time_signature_confidence = (decimal)result.Analysis.time_signature_confidence,
                            key = result.Analysis.key,
                            key_confidence = (decimal)result.Analysis.key_confidence,
                            mode = result.Analysis.mode,
                            mode_confidence = (decimal)result.Analysis.mode_confidence
                        };
                        result.Bars.ForEach(b => {
                            var bar = new TrackBar()
                            {
                                start = (decimal)b.Start,
                                duration = (decimal)b.Duration,
                                confidence = (decimal)b.Confidence
                            };
                            track.audioAnalysis.bars.Add(bar);
                        });
                        result.Beats.ForEach(b => {
                            var beat = new TrackBeat()
                            {
                                start = (decimal)b.Start,
                                duration = (decimal)b.Duration,
                                confidence = (decimal)b.Confidence
                            };
                            track.audioAnalysis.beats.Add(beat);
                        });
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
                            track.audioAnalysis.sections.Add(section);
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
                            track.audioAnalysis.segments.Add(segment);
                        });
                        result.Tatums.ForEach(t => {
                            var tatum = new TrackTatum()
                            {
                                start = (decimal)t.Start,
                                duration = (decimal)t.Duration,
                                confidence = (decimal)t.Confidence
                            };
                            track.audioAnalysis.tatums.Add(tatum);
                        });
                    }

                });
                album.tracks.Add(track);
            });
        }
        public List<TrackAudioFeatures> GetAudioFeatures(List<string> ids)
        {
            var trackFeatures = new List<TrackAudioFeatures>();
            for (int i = 0; i < ids.Count; i += (ids.Count - i) >= 50 ? 50 : (ids.Count - i))
            {
                var request = new TracksAudioFeaturesRequest(ids.GetRange(i, (ids.Count - i) >= 50 ? 50 : (ids.Count - i)));
                var response = client.Tracks.GetSeveralAudioFeatures(request).Result;

                trackFeatures.AddRange(response.AudioFeatures.ToList());
            }
            return trackFeatures;
        }
        public void GetTrackAudioFeatures(ref Artist artist)
        {
            var ids = new List<string>();
            artist.albums.ForEach(album => {
                ids.AddRange(album.tracks.Select(track => track.id).ToList());
            });
            var trackFeatures = new Dictionary<string, TrackFeatures>();
            for (int i = 0; i < ids.Count; i += (ids.Count - i) >= 50 ? 50 : (ids.Count - i))
            {
                var request = new TracksAudioFeaturesRequest(ids.GetRange(i, (ids.Count - i) >= 50 ? 50 : (ids.Count - i)));
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
                    trackFeatures.TryAdd(f.Id, trackFeature);
                });
                artist.albums.ForEach(album => {
                    album.tracks.ForEach(track => {
                        track.features = trackFeatures.Where(f => f.Key == track.id).FirstOrDefault().Value;
                    });
                });
            }
        }            
    //    public List<Track> GetTrackAudioAnalysis(List<Track> tracks)
    //    {
    //        var ids = tracks.Select(t => t.id).ToList();
    //        tracks.ForEach(track => {
    //            var result = _client.Tracks.GetAudioAnalysis(track.id).Result;
               
    //            var trackAudioAnalysis = new TrackAudioAnalysis()
    //            {
    //                num_samples = result.Track.num_samples,
    //                duration = (decimal)result.Track.duration,
    //                sample_md5 = result.Track.sample_md5,
    //                offset_seconds = result.Track.offset_seconds,
    //                window_seconds = result.Track.window_seconds,
    //                analysis_sample_rate = result.Track.analysis_sample_rate,
    //                analysis_channels = result.Track.analysis_channels,
    //                end_of_fade_in = (decimal)result.Track.end_of_fade_in,
    //                start_of_fade_out = (decimal)result.Track.start_of_fade_out,
    //                loudness = (decimal)result.Track.loudness,
    //                tempo = (decimal)result.Track.tempo,
    //                tempo_confidence = (decimal)result.Track.tempo_confidence,
    //                time_signature = result.Track.time_signature,
    //                time_signature_confidence = (decimal)result.Track.time_signature_confidence,
    //                key = result.Track.key,
    //                key_confidence = (decimal)result.Track.key_confidence,
    //                mode = result.Track.mode,
    //                mode_confidence = (decimal)result.Track.mode_confidence
    //            };

    //            result.Sections.ForEach(s => {
    //                var section = new TrackSection()
    //                {
    //                    start = (decimal)s.Start,
    //                    duration = (decimal)s.Duration,
    //                    confidence = (decimal)s.Confidence,
    //                    loudness = (decimal)s.Loudness,
    //                    tempo = (decimal)s.Tempo,
    //                    tempo_confidence = (decimal)s.TempoConfidence,  
    //                    key = s.Key,
    //                    key_confidence = (decimal)s.KeyConfidence,
    //                    mode = s.Mode,
    //                    mode_confidence = (decimal)s.ModeConfidence,
    //                    time_signature = s.TimeSignature,
    //                    time_signature_confidence = (decimal)s.TimeSignatureConfidence
    //                };
    //                trackAudioAnalysis.sections.Add(section);
    //            });

    //            result.Segments.ForEach(s => {
    //                var segment = new TrackSegment()
    //                {
    //                    start = (decimal)s.Start,
    //                    duration = (decimal)s.Duration,
    //                    confidence = (decimal)s.Confidence,
    //                    loudness_start = (decimal)s.LoudnessStart,
    //                    loudness_max_time = (decimal)s.LoudnessMaxTime, 
    //                    loudness_max = (decimal)s.LoudnessMax,
    //                    loudness_end = (decimal)s.LoudnessEnd
    //                };
    //                s.Pitches.ForEach(pitch => { segment.pitches.Add(new Pitch() { pitch = (decimal)pitch }); });
    //                s.Timbre.ForEach(timbre => { segment.timbres.Add(new Timbre() { timbre = (decimal)timbre }); });
    //                trackAudioAnalysis.segments.Add(segment);
    //            });

    //            result.Tatums.ForEach(t => {
    //                var tatum = new TrackTatum() { 
    //                    start = (decimal)t.Start,
    //                    duration = (decimal)t.Duration,
    //                    confidence = (decimal)t.Confidence
    //                };
    //                trackAudioAnalysis.tatums.Add(tatum);
    //            });

    //            track.audioAnalysis = trackAudioAnalysis;
    //        });
    //        return tracks;
    //    }
    //    public List<Track> GetTrackAudioFeatures(List<Track> tracks)
    //    {
    //        //var trackFeatures = new TrackFeatures();
    //        var ids = tracks.Select(ct => ct.id).ToList();
    //        for (int i = 0; i < ids.Count; i += (ids.Count - i) >= 50 ? 50 : (ids.Count - i))
    //        {
    //            var request = new TracksAudioFeaturesRequest(ids.GetRange(i, (ids.Count - i) >= 50 ? 50 : (ids.Count - i)));
    //            _client.Tracks.GetSeveralAudioFeatures(request).Result.AudioFeatures.ForEach(f =>
    //            {
    //                var trackFeatures = new TrackFeatures()
    //                {
    //                    danceability = (decimal)f.Danceability,
    //                    energy = (decimal)f.Energy,
    //                    key = f.Key,
    //                    loudness = (decimal)f.Loudness,
    //                    mode = f.Mode,
    //                    speechiness = (decimal)f.Speechiness,
    //                    acousticness = (decimal)f.Acousticness,
    //                    instrumentalness = (decimal)f.Instrumentalness,
    //                    liveness = (decimal)f.Liveness,
    //                    valence = (decimal)f.Valence,
    //                    tempo = (decimal)f.Tempo,
    //                    duration_ms = f.DurationMs,
    //                    time_signature = f.TimeSignature
    //                };
    //                tracks.Where(t => t.id == f.Id).FirstOrDefault().features = trackFeatures;
    //            });

    //        }
    //        return tracks;
    //    }
    //    public List<ChartReportTrack> UpdateChartReportTrackArtistId(List<Track> tracks, List<ChartReportTrack> chartReportTracks)
    //    {
    //        var query = from chart in chartReportTracks
    //                    join track in tracks on chart.spotifyTrackId equals track.id
    //                    select new ChartReportTrack()
    //                    {
    //                        position = chart.position,
    //                        trackName = chart.trackName,
    //                        artist = chart.artist,
    //                        streams = chart.streams,
    //                        url = chart.url,
    //                    };
    //        return query.ToList();
    //    }

    //    public Dictionary<string, FullArtist> GetTrackArtists(List<FullTrack> fullTracks)
    //    {
    //        var ids = new List<string>();
    //        fullTracks.ForEach(ft => ids.AddRange(ft.Artists.Select(a => a.Id).ToList()));
    //        var artists = new Dictionary<string, FullArtist>();
    //        for (int i = 0; i < ids.Count; i += (ids.Count - i) >= 50 ? 50 : (ids.Count - i))
    //        {
    //            var response = _client.Artists.GetSeveral(new ArtistsRequest(ids.GetRange(i, ((ids.Count - i) < 50 ? (ids.Count - i) : 50)))).Result.Artists;
    //            response.ForEach(a => {
    //                artists.TryAdd(a.Id, a);
    //            });
    //        }
    //        return artists;
    //    }
    //    public List<Artist> GetArtistRelatedArtists(List<Artist> artists)
    //    {
    //        foreach (var artist in artists)
    //        {
    //            try { 
    //                var response = _client.Artists.GetRelatedArtists(artist.id).Result.Artists;
    //                response.ForEach(a => {
    //                    var relatedArtist = new RelatedArtist() { Id = a.Id };
    //                    artist.relatedArtists.Add(relatedArtist);
    //                });              
    //            }
    //            catch(Exception ex)
    //            {

    //            }
    //        }
    //        return artists;
    //    }
    //    public List<Artist> GetAlbumTracks(List<Artist> artists)
    //    {
    //        try
    //        {
    //            artists.ForEach(artist => {
    //                artist.albums.ForEach(album => { 
    //                    var response = _client.Albums.GetTracks(album.id).Result.Items.ToList();
    //                    response.ForEach(t => {
    //                        var track = new Track()
    //                        {
    //                            duration_ms = t.DurationMs,
    //                            isexplicit = t.Explicit,
    //                            name = t.Name,
    //                            preview_url = t.PreviewUrl,
    //                            track_number = t.TrackNumber,
    //                            type = t.Type.ToString(),
    //                            uri = t.Uri,
    //                            spotify_url = t.ExternalUrls.Values.FirstOrDefault(),
    //                            id = t.Id,
    //                        };
    //                        album.tracks.Add(track);
    //                    });
    //                });
    //            });
    //        }
    //        catch(Exception ex)
    //        {

    //        }
    //        return artists;
    //    }
    //    public void JoinArtistData(Dictionary<string, FullArtist> artists,
    //                         Dictionary<string, List<FullArtist>> relatedArtists,
    //                         Dictionary<string, List<SimpleAlbum>> artistAlbums,
    //                         Dictionary<string, List<SimpleTrack>> albumTracks)
    //    {


    //        var query = from a in artists
    //                    join ra in relatedArtists on a.Key equals ra.Key
    //                    join aa in artistAlbums on a.Key equals aa.Key
    //                    select new Artist
    //                    {
    //                        id = a.Key,
    //                        name = a.Value.Name,
    //                        popularity = a.Value.Popularity,
    //                        type = a.Value.Type,
    //                        uri = a.Value.Uri,
    //                        followers = a.Value.Followers.Total,
    //                        spotify_url = a.Value.ExternalUrls.FirstOrDefault().Value,
    //                        relatedArtists = ra.Value.Select(ra => new RelatedArtist() { Id = ra.Id}).ToList(),
    //                        albums = (from album in aa.Value
    //                                 join tracks in albumTracks on album.Id equals tracks.Key
    //                                 select new Album
    //                                 {
    //                                     id = album.Id,
    //                                     album_group = album.AlbumGroup,
    //                                     album_type = album.AlbumType,
    //                                     name = album.Name,
    //                                     release_date = album.ReleaseDate,
    //                                     release_date_precision = album.ReleaseDatePrecision,
    //                                     total_tracks = tracks.Value.Count,
    //                                     type = album.Type,
    //                                     uri = album.Uri,
    //                                     tracks = tracks.Value.Select(t => new Track() {
    //                                         duration_ms = t.DurationMs,
    //                                         isexplicit = t.Explicit,
    //                                         name = t.Name,
    //                                         preview_url = t.PreviewUrl,
    //                                         track_number = t.TrackNumber,
    //                                         //type = t.Type
    //                                         uri = t.Uri,
    //                                         spotify_url = t.ExternalUrls.Values.FirstOrDefault(),
    //                                         id = t.Id
    //                                     }).ToList()

    //                                 }).ToList()
    //                    };

    //        query.ToList();
            

    //    } 
    }
}
