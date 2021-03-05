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


            var chartReport = spotify.GetChartTracks(countryCode, weekStart, weekEnd, 0, 5);
            
            //var (artistIds, albumIds, chartTracks) = spotify.GetChartData(chartReport, countryCode, weekStart, weekEnd);
            var (artists, chartTracks) = spotify.GetChartData(chartReport, countryCode, weekStart, weekEnd);

            spotify.RefreshToken();

            ////Get full artist 
            //var fullArtists = spotify.GetSeveralArtistsFull(artistIds.Distinct().ToList());

            ////Get albums
            //var atistIsa2 = artistIds.Distinct().ToList();
            //var artistAlbums = spotify.GetArtistAlbums(atistIsa2);
            //var artistAlbumIds = artistAlbums.Select(a => a.Id).ToList();
            //albumIds.AddRange(artistAlbumIds);

            //var fullAlbums = spotify.GetFullAlbums(albumIds.Where(a => !string.IsNullOrEmpty(a)).Distinct().ToList());

            ////Get tracks
            //var trackIds = new List<string>();
            //fullAlbums.ForEach(a => 
            //{
            //    try
            //    {
            //        a.Tracks.Items.ForEach(i =>
            //        {
            //            try
            //            {
            //                trackIds.Add(i.Id);
                            
            //            }
            //            catch (Exception e)
            //            {
            //            }
            //        });
            //    }
            //    catch (Exception e) 
            //    {
            //    }

            //});


            //var audioFeatures = spotify.GetAudioFeatures(trackIds.Distinct().ToList());
            //var audioAnalysis = spotify.GetTrackAudioAnalysis(trackIds.Distinct().ToList());

            //var x = BuildArtistList(fullArtists, fullAlbums, audioFeatures, audioAnalysis);
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
