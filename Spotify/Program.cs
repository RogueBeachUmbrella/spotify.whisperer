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
using Spotify.SpotifyModels;

namespace Spotify
{
    class Program
    {
        static void Main(string[] args)
        {
            var spotify = new Spotify();
            var postgres = new Postgres();

            string countryCode = "us";
            DateTime weekStart = new DateTime(2021, 02, 05);
            DateTime weekEnd = new DateTime(2021, 02, 12);


            var chartReport = spotify.GetChartTracks(countryCode, weekStart, weekEnd);
            var fullTracks = spotify.GetChartTrackData(chartReport);


            var artists = GetArtistData(spotify, fullTracks);
            var (albums, tracks) = GetAlbumTrackData(spotify, fullTracks, artists.Select(a => a.Id).Distinct().ToList());
            var audioFeatures = spotify.GetAudioFeatures(tracks.Select(t => t.Id).Distinct().ToList());
            var audioAnalysis = spotify.GetTrackAudioAnalysis(tracks.Select(t => t.Id).Distinct().ToList());
            
            MapTrackFeatures(tracks, audioFeatures, audioAnalysis);


            postgres.LoadArtists(artists);
            postgres.LoadArtistGenres(artists);
            postgres.LoadAlbums(albums);
            postgres.LoadAlbumArtists(albums);
            postgres.LoadTracks(tracks);
            postgres.LoadTrackAlbum(albums);
            postgres.LoadTrackArtists(tracks);







            var i = 1;



            /*


            var x = BuildArtistList(fullArtists, fullAlbums, audioFeatures, audioAnalysis);
            */
        }

        private static void MapTrackFeatures(List<SpotifyModels.Track> tracks, List<TrackAudioFeatures> audioFeatures, Dictionary<string, TrackAudioAnalysis> audioAnalysis)
        {
            tracks.ForEach(t =>
            {
                t.audioFeatures = audioFeatures.Where(f => f != null && f.Id == t.Id).Select(f =>
                new AudioFeature
                {
                    Acousticness = f.Acousticness,
                    AnalysisUrl = f.AnalysisUrl,
                    Danceability = f.Danceability,
                    DurationMs = f.DurationMs,
                    Energy = f.Energy,
                    Instrumentalness = f.Instrumentalness,
                    Key = f.Key,
                    Liveness = f.Liveness,
                    Loudness = f.Loudness,
                    Mode = f.Mode,
                    Speechiness = f.Speechiness,
                    Tempo = f.Tempo,
                    TimeSignature = f.TimeSignature,
                    TrackHref = f.TrackHref,
                    Type = f.Type,
                    Uri = f.Uri,
                    Valence = f.Valence
                }).FirstOrDefault();

                t.audioAnalysis = audioAnalysis.Where(a => a.Key == t.Id).Select(f =>
                new AudioAnalysis
                {
                    Meta = new SpotifyModels.Meta()
                    {
                        analyzer_version = f.Value.Meta.analyzer_version,
                        platform = f.Value.Meta.platform,
                        detailed_status = f.Value.Meta.detailed_status,
                        status_code = f.Value.Meta.status_code,
                        timestamp = f.Value.Meta.timestamp,
                        analysis_time = f.Value.Meta.analysis_time,
                        input_process = f.Value.Meta.input_process
                    },
                    Analysis = new SpotifyModels.Analysis()
                    {
                        num_samples = f.Value.Track.num_samples,
                        duration = f.Value.Track.duration,
                        sample_md5 = f.Value.Track.sample_md5,
                        offset_seconds = f.Value.Track.offset_seconds,
                        window_seconds = f.Value.Track.window_seconds,
                        analysis_sample_rate = f.Value.Track.analysis_sample_rate,
                        analysis_channels = f.Value.Track.analysis_channels,
                        end_of_fade_in = f.Value.Track.end_of_fade_in,
                        start_of_fade_out = f.Value.Track.start_of_fade_out,
                        loudness = f.Value.Track.loudness,
                        tempo = f.Value.Track.tempo,
                        tempo_confidence = f.Value.Track.tempo_confidence,
                        time_signature = f.Value.Track.time_signature,
                        time_signature_confidence = f.Value.Track.time_signature_confidence,
                        key = f.Value.Track.key,
                        key_confidence = f.Value.Track.key_confidence,
                        mode = f.Value.Track.mode,
                        mode_confidence = f.Value.Track.mode_confidence
                    },
                    Bars = f.Value.Bars.Select(b => new SpotifyModels.TimeInterval()
                    {
                        Start = b.Start,
                        Duration = b.Duration,
                        Confidence = b.Confidence
                    }).ToList(),
                    Beats = f.Value.Beats.Select(b => new SpotifyModels.TimeInterval()
                    {
                        Start = b.Start,
                        Duration = b.Duration,
                        Confidence = b.Confidence
                    }).ToList(),
                    Sections = f.Value.Sections.Select(s => new SpotifyModels.Section()
                    {
                        Start = s.Start,
                        Duration = s.Duration,
                        Confidence = s.Confidence,
                        Loudness = s.Loudness,
                        Tempo = s.Tempo,
                        TempoConfidence = s.TempoConfidence,
                        Key = s.Key,
                        KeyConfidence = s.KeyConfidence,
                        Mode = s.Mode,
                        ModeConfidence = s.ModeConfidence,
                        TimeSignature = s.TimeSignature,
                        TimeSignatureConfidence = s.TimeSignatureConfidence
                    }).ToList(),
                    Segments = f.Value.Segments.Select(s => new SpotifyModels.Segment()
                    {
                        Start = s.Start,
                        Duration = s.Duration,
                        Confidence = s.Confidence,
                        LoudnessStart = s.LoudnessStart,
                        LoudnessMax = s.LoudnessMax,
                        LoudnessMaxTime = s.LoudnessMaxTime,
                        LoudnessEnd = s.LoudnessEnd,
                        Pitches = s.Pitches,
                        Timbre = s.Timbre,
                    }).ToList(),
                    Tatums = f.Value.Tatums.Select(b => new SpotifyModels.TimeInterval()
                    {
                        Start = b.Start,
                        Duration = b.Duration,
                        Confidence = b.Confidence
                    }).ToList()
                }).FirstOrDefault();
            });
        }

        //private static List<SpotifyModels.Track> GetTracks(List<FullTrack> fullTracks, List<FullAlbum> artistFullAlbums)
        //{
        //    var simpleTracks = artistFullAlbums.SelectMany(x => x.Tracks.Items).Distinct().ToList();

        //    return (from st in simpleTracks
        //                 join f in fullTracks on st.Id equals f.Id into gj
        //                 from ft in gj.DefaultIfEmpty()
        //                 select new SpotifyModels.Track()
        //                 {
        //                     Id = ft?.Id ?? st.Id,
        //                     Name = ft?.Name ?? st.Name,
        //                     Popularity = ft != null ? ft.Popularity : 0,
        //                     DiscNumber = ft != null ? ft.DiscNumber : st.DiscNumber,
        //                     DurationMs = st.DurationMs,
        //                     Explicit = st.Explicit,
        //                     Href = st.Href,
        //                     PreviewUrl = st.PreviewUrl,
        //                     TrackNumber = st.TrackNumber,
        //                     artistIds = st.Artists.Select(a => a.Id).ToList()

        //                 }).ToList();
        //}

        private static List<SpotifyModels.Album> GetAllAlbums(IEnumerable<SimpleAlbum> allSimpleAlbums)
        {
            return allSimpleAlbums.Select(album => new SpotifyModels.Album()
            {
                Id = album.Id,
                AlbumType = album.AlbumType,
                Name = album.Name,
                ReleaseDate = album.ReleaseDate,
                ReleaseDatePrecision = album.ReleaseDatePrecision,
                Type = album.Type,
                Uri = album.Uri,
                ExternalUrl = album.ExternalUrls.FirstOrDefault().Value,
                Href = album.Href,
                Images = album.Images,
                artistIds = album.Artists.Select(a => a.Id).ToList()
            }).ToList();
        }

        private static (List<SpotifyModels.Album> albums, List<SpotifyModels.Track> tracks) GetAlbumTrackData(Spotify spotify, List<FullTrack> fullTracks, List<string> artistIds)
        {

            var artistSimpleAlbums = spotify.GetArtistAlbums(artistIds); //missing singles

            var albumIds = artistSimpleAlbums.Select(x => x.Id).Distinct().ToList();

            var artistFullAlbums = spotify.GetFullAlbums(albumIds);

            var albums = artistFullAlbums.Select(album =>
            new SpotifyModels.Album()
            {
                Id = album.Id,
                AlbumType = album.AlbumType,
                Name = album.Name,
                ReleaseDate = album.ReleaseDate,
                ReleaseDatePrecision = album.ReleaseDatePrecision,
                Type = album.Type,
                Uri = album.Uri,
                ExternalUrl = album.ExternalUrls.FirstOrDefault().Value,
                Href = album.Href,
                Images = album.Images,
                artistIds = album.Artists.Select(a => a.Id).ToList(),
                trackIds = album.Tracks.Items.Select(a => a.Id).ToList()
            }).ToList();


            albums.AddRange(
                fullTracks.Where(a => !albums.Exists(b => b.Id == a.Album.Id)).Select(track =>
                new SpotifyModels.Album()
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
                    artistIds = track.Album.Artists.Select(a => a.Id).ToList(),
                    trackIds = new List<string>() { track.Id}
                }).ToList()
            );

            var tracks = fullTracks.Select(t =>
            new SpotifyModels.Track()
            {
                Id = t.Id,
                Name = t.Name,
                Popularity = t.Popularity,
                TrackNumber = t.TrackNumber,
                DiscNumber = t.DiscNumber,
                Explicit = t.Explicit,
                Href = t.Href,
                PreviewUrl = t.PreviewUrl,
                DurationMs = t.DurationMs
            }).ToList();


            tracks.AddRange(artistFullAlbums.SelectMany(a => a.Tracks.Items).ToList().Where(a => !tracks.Exists(t => t.Id == a.Id)).Select(t => new SpotifyModels.Track()
            {
                Id = t.Id,
                Name = t.Name,
                TrackNumber = t.TrackNumber,
                DiscNumber = t.DiscNumber,
                Explicit = t.Explicit,
                Href = t.Href,
                PreviewUrl = t.PreviewUrl,
                DurationMs = t.DurationMs
            }).ToList());

            return (albums, tracks);
        }

        private static List<Artist> GetArtistData(Spotify spotify, List<FullTrack> fullTracks)
        {
            List<string> artistIds;
            List<Artist> artists;

            artistIds = fullTracks.SelectMany(x => x.Artists.Select(a => a.Id)).Distinct().ToList();
            var fullArtists = spotify.GetSeveralArtistsFull(artistIds);


            //create master artist list
            artists = fullArtists.Select(artist => new SpotifyModels.Artist()
            {
                Id = artist.Id,
                Name = artist.Name,
                Type = artist.Type,
                Uri = artist.Uri,
                ExternalUrl = artist.ExternalUrls.Values.FirstOrDefault(),
                Href = artist.Href
            }).Distinct().ToList();

            return artists;
        }

        static List<SpotifyModels.Artist> BuildArtistList(List<FullArtist> fullArtists, List<FullAlbum> albums, List<TrackAudioFeatures> audioFeatures, Dictionary<string, SpotifyAPI.Web.TrackAudioAnalysis> audioAnalysis)
        {
            var artists = new List<Artist>();

            var query = from artist in fullArtists
                        select new SpotifyModels.Artist()
                        {
                            ExternalUrl = artist.ExternalUrls.FirstOrDefault().Value,
                            Followers = artist.Followers.Total,
                            Genres = artist.Genres,
                            Href = artist.Href,
                            Id = artist.Id,
                            Images = artist.Images,
                            Name = artist.Name,
                            Popularity = artist.Popularity,
                            Type = artist.Type,
                            Uri = artist.Uri,
                            albums = (from album in albums.Where(a => a!=null)
                                      where album.Artists.Select(a => a.Id == artist.Id).FirstOrDefault()
                                      select new Album()
                                      {
                                          AlbumType = album.AlbumType,
                                          ExternalUrl = album.ExternalUrls.FirstOrDefault().Value,
                                          Genres = album.Genres,
                                          Href = album.Href,
                                          Id = album.Href,
                                          Images = album.Images,
                                          Name = album.Name,
                                          Popularity = album.Popularity,
                                          ReleaseDate = album.ReleaseDate,
                                          ReleaseDatePrecision = album.ReleaseDatePrecision,
                                          Type = album.Type,
                                          Uri = album.Uri,
                                          Tracks = album.Tracks.Items.Select(track => new SpotifyModels.Track()
                                          {
                                              DiscNumber = track.DiscNumber,
                                              DurationMs = track.DurationMs,
                                              Explicit = track.Explicit,
                                              Href = track.Href,
                                              Id = track.Id,
                                              Name = track.Name,
                                              PreviewUrl = track.PreviewUrl,
                                              TrackNumber = track.TrackNumber,
                                              audioFeatures = (from feature in audioFeatures
                                                               where feature.Id == track.Id
                                                               select new AudioFeature()
                                                               {
                                                                   Acousticness = feature.Acousticness,
                                                                   AnalysisUrl = feature.AnalysisUrl,
                                                                   Danceability = feature.Danceability,
                                                                   DurationMs = feature.DurationMs,
                                                                   Energy = feature.Energy,
                                                                   Instrumentalness = feature.Instrumentalness,
                                                                   Key = feature.Key,
                                                                   Liveness = feature.Liveness,
                                                                   Loudness = feature.Loudness,
                                                                   Mode = feature.Mode,
                                                                   Speechiness = feature.Speechiness,
                                                                   Tempo = feature.Tempo,
                                                                   TimeSignature = feature.TimeSignature,
                                                                   TrackHref = feature.TrackHref,
                                                                   Type = feature.Type,
                                                                   Uri = feature.Uri,
                                                                   Valence = feature.Valence
                                                               }).FirstOrDefault(),
                                              audioAnalysis = (from analysis in audioAnalysis
                                                               where analysis.Key == track.Id
                                                               select new SpotifyModels.AudioAnalysis()
                                                               {
                                                                   Meta = new SpotifyModels.Meta()
                                                                   {
                                                                       analyzer_version = analysis.Value.Meta.analyzer_version,
                                                                       platform = analysis.Value.Meta.platform,
                                                                       detailed_status = analysis.Value.Meta.detailed_status,
                                                                       status_code = analysis.Value.Meta.status_code,
                                                                       timestamp = analysis.Value.Meta.timestamp,
                                                                       analysis_time = analysis.Value.Meta.analysis_time,
                                                                       input_process = analysis.Value.Meta.input_process
                                                                   },
                                                                   //Analysis = new SpotifyModels.Analysis() {
                                                                   //   num_samples = analysis.Value.Analysis.num_samples
                                                                   //},
                                                                   //{
                                                                   //num_samples = analysis.Value.Analysis.num_samples
                                                                   //duration = analysis.Value.Analysis.duration,
                                                                   //sample_md5 = analysis.Value.Analysis.sample_md5,
                                                                   //offset_seconds = analysis.Value.Analysis.offset_seconds,
                                                                   //window_seconds = analysis.Value.Analysis.window_seconds,
                                                                   //analysis_sample_rate = analysis.Value.Analysis.analysis_sample_rate,
                                                                   //analysis_channels = analysis.Value.Analysis.analysis_channels,
                                                                   //end_of_fade_in = analysis.Value.Analysis.end_of_fade_in,
                                                                   //start_of_fade_out = analysis.Value.Analysis.start_of_fade_out,
                                                                   //loudness = analysis.Value.Analysis.loudness,
                                                                   //tempo = analysis.Value.Analysis.tempo,
                                                                   //tempo_confidence = analysis.Value.Analysis.tempo_confidence,
                                                                   //time_signature = analysis.Value.Analysis.time_signature,
                                                                   //time_signature_confidence = analysis.Value.Analysis.time_signature_confidence,
                                                                   //key = analysis.Value.Analysis.key,
                                                                   //key_confidence = analysis.Value.Analysis.key_confidence,
                                                                   //mode = analysis.Value.Analysis.mode,
                                                                   //mode_confidence = analysis.Value.Analysis.mode_confidence

                                                                   //},
                                                                   Bars = analysis.Value.Beats.Select(b => new SpotifyModels.TimeInterval()
                                                                   {
                                                                       Start = b.Start,
                                                                       Duration = b.Duration,
                                                                       Confidence = b.Confidence
                                                                   }).ToList(),
                                                                   Beats = analysis.Value.Beats.Select(b => new SpotifyModels.TimeInterval()
                                                                   {
                                                                       Start = b.Start,
                                                                       Duration = b.Duration,
                                                                       Confidence = b.Confidence
                                                                   }).ToList(),
                                                                   Sections = analysis.Value.Sections.Select(s => new SpotifyModels.Section()
                                                                   {
                                                                       Start = s.Start,
                                                                       Duration = s.Duration,
                                                                       Confidence = s.Confidence,
                                                                       Loudness = s.Loudness,
                                                                       Tempo = s.Tempo,
                                                                       TempoConfidence = s.TempoConfidence,
                                                                       Key = s.Key,
                                                                       KeyConfidence = s.KeyConfidence,
                                                                       Mode = s.Mode,
                                                                       ModeConfidence = s.ModeConfidence,
                                                                       TimeSignature = s.TimeSignature,
                                                                       TimeSignatureConfidence = s.TimeSignatureConfidence
                                                                   }).ToList(),
                                                                   Segments = analysis.Value.Segments.Select(s => new SpotifyModels.Segment()
                                                                   {
                                                                       Start = s.Start,
                                                                       Duration = s.Duration,
                                                                       Confidence = s.Confidence,
                                                                       LoudnessStart = s.LoudnessStart,
                                                                       LoudnessMax = s.LoudnessMax,
                                                                       LoudnessMaxTime = s.LoudnessMaxTime,
                                                                       LoudnessEnd = s.LoudnessEnd

                                                                   }).ToList(),
                                                                   Tatums = analysis.Value.Tatums.Select(b => new SpotifyModels.TimeInterval()
                                                                   {
                                                                       Start = b.Start,
                                                                       Duration = b.Duration,
                                                                       Confidence = b.Confidence
                                                                   }).ToList()
                                                               }).FirstOrDefault()

                                          }).ToList()
                                      }).ToList()

                        };
            return query.ToList();
        }
    }
}
