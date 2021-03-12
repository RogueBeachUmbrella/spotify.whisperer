using System;
using System.Collections.Generic;
using SpotifyAPI.Web;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Data;
using Spotify.SpotifyModels;
using System.Configuration;
using MongoDB.Driver;

namespace Spotify
{
    class Program
    {
        static void Main(string[] args)
        {

            var spotify = new Spotify();
            var postgres = new Postgres();
            var mongo = new MongoClient("");

            string countryCode = "us";
            DateTime weekStart = new DateTime(2021, 02, 05);
            DateTime weekEnd = new DateTime(2021, 02, 12);

            SpotifyReport Top200Tracks = spotify.GetChartTracks(countryCode, weekStart, weekEnd, 1, 1);

            IMongoDatabase Spotify = mongo.GetDatabase("Spotify");

            List<FullTrack> fullTracks = spotify.GetFullTracks(Top200Tracks.tracks.Select(t => t.id).ToList());
            List<FullArtist> fullArtists = spotify.GetSeveralArtistsFull(fullTracks.SelectMany(x => x.Artists.Select(a => a.Id)).Distinct().ToList());







            List<Artist> artists = fullArtists.Select(artist => new Artist()
            {
                Id = artist.Id,
                Name = artist.Name,
                Type = artist.Type,
                Uri = artist.Uri,
                ExternalUrl = artist.ExternalUrls.Values.FirstOrDefault(),
                Followers = artist.Followers.Total,
                Images = artist.Images,
                Popularity = artist.Popularity,
                albums = fullTracks.Where(ft => ft.Album.Artists.Exists(a => a.Id == artist.Id)).Select(ft => ft.Album.Id).ToList(),
                tracks = fullTracks.Where(ft => ft.Artists.Exists(a => a.Id == artist.Id)).Select(ft => ft.Id).ToList(),
                Genres = artist.Genres.ToList()
            }).Distinct().ToList();

            List<SimpleAlbum> artistSimpleAlbums = spotify.GetArtistAlbums(artists.Select(a => a.Id).ToList()); //missing singles

            List<FullAlbum> artistFullAlbums = spotify.GetFullAlbums(artistSimpleAlbums.Select(x => x.Id).Distinct().ToList());

            List<Album> albums = artistFullAlbums.Select(album => new Album() 
            {
                Id = album.Id,
                AlbumType = album.AlbumType,
                Name = album.Name,
                ReleaseDate = album.ReleaseDate,
                ReleaseDatePrecision = album.ReleaseDatePrecision,
                Type = album.Type,
                Uri = album.Uri,
                ExternalUrl = album.ExternalUrls.FirstOrDefault().Value,
                Images = album.Images,
                artistIds = album.Artists.Select(a => a.Id).ToList(),
                trackIds = album.Tracks.Items.Select(a => a.Id).ToList(),
                Genres = album.Genres.ToList()
            })  .ToList()
                .Concat(fullTracks
                .Where(track => !artistFullAlbums
                .Exists(album => album.Id == track.Album.Id))
                .Select(track =>
            new Album()
            {
                Id = track.Album.Id,
                AlbumType = track.Album.AlbumType,
                Name = track.Album.Name,
                ReleaseDate = track.Album.ReleaseDate,
                ReleaseDatePrecision = track.Album.ReleaseDatePrecision,
                Type = track.Album.Type,
                Uri = track.Album.Uri,
                ExternalUrl = track.Album.ExternalUrls.FirstOrDefault().Value,
                artistIds = track.Album.Artists.Select(a => a.Id).ToList(),
                trackIds = new List<string>() { track.Id }
            }).ToList()).ToList();

            List<Track> tracks = fullTracks.Select(t =>
            new Track()
            {
                Id = t.Id,
                Name = t.Name,
                Popularity = t.Popularity,
                TrackNumber = t.TrackNumber,
                DiscNumber = t.DiscNumber,
                Explicit = t.Explicit,
                PreviewUrl = t.PreviewUrl,
                DurationMs = t.DurationMs,
                artistIds = t.Artists.Select(a => a.Id).ToList(),
                albumId = t.Album.Id
            })
                .ToList()
                .Concat(artistFullAlbums
                .SelectMany(fullAlbum => fullAlbum.Tracks.Items)
                .ToList()
                .Where(simpleTrack => !fullTracks
                .Exists(fullTrack => fullTrack.Id == simpleTrack.Id))
                .Select(track => new SpotifyModels.Track()
                {
                    Id = track.Id,
                    Name = track.Name,
                    TrackNumber = track.TrackNumber,
                    DiscNumber = track.DiscNumber,
                    Explicit = track.Explicit,
                    PreviewUrl = track.PreviewUrl,
                    DurationMs = track.DurationMs,
                    artistIds = track.Artists.Select(a => a.Id).ToList(),
                    albumId = artistFullAlbums.Where(a => a.Tracks.Items.Exists(t => t.Id == track.Id)).Select(a => a.Id).FirstOrDefault()

                })
                .ToList())
                .ToList();

            List<TrackAudioFeatures> audioFeatures = spotify.GetAudioFeatures(tracks.Select(t => t.Id).Distinct().ToList());
            List<TrackAudioAnalysis> audioAnalysis = spotify.GetTrackAudioAnalysis(tracks.Select(t => t.Id).Distinct().ToList());

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
                    Valence = f.Valence
                }).FirstOrDefault();

                t.audioAnalysis = audioAnalysis.Where(a => !(a == null || !a.TrackId.Equals(t.Id))).Select(f =>
                new AudioAnalysis
                {
                    Meta = new SpotifyModels.Meta()
                    {
                        analyzer_version = f.Meta.analyzer_version,
                        platform = f.Meta.platform,
                        detailed_status = f.Meta.detailed_status,
                        status_code = f.Meta.status_code,
                        timestamp = f.Meta.timestamp,
                        analysis_time = f.Meta.analysis_time,
                        input_process = f.Meta.input_process
                    },
                    Analysis = new SpotifyModels.Analysis()
                    {
                        num_samples = f.Track.num_samples,
                        duration = f.Track.duration,
                        sample_md5 = f.Track.sample_md5,
                        offset_seconds = f.Track.offset_seconds,
                        window_seconds = f.Track.window_seconds,
                        analysis_sample_rate = f.Track.analysis_sample_rate,
                        analysis_channels = f.Track.analysis_channels,
                        end_of_fade_in = f.Track.end_of_fade_in,
                        start_of_fade_out = f.Track.start_of_fade_out,
                        loudness = f.Track.loudness,
                        tempo = f.Track.tempo,
                        tempo_confidence = f.Track.tempo_confidence,
                        time_signature = f.Track.time_signature,
                        time_signature_confidence = f.Track.time_signature_confidence,
                        key = f.Track.key,
                        key_confidence = f.Track.key_confidence,
                        mode = f.Track.mode,
                        mode_confidence = f.Track.mode_confidence
                    },
                    Bars = f.Bars.Select(b => new SpotifyModels.TimeInterval()
                    {
                        Start = b.Start,
                        Duration = b.Duration,
                        Confidence = b.Confidence
                    }).ToList(),
                    Beats = f.Beats.Select(b => new SpotifyModels.TimeInterval()
                    {
                        Start = b.Start,
                        Duration = b.Duration,
                        Confidence = b.Confidence
                    }).ToList(),
                    Sections = f.Sections.Select(s => new SpotifyModels.Section()
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
                    Segments = f.Segments.Select(s => new SpotifyModels.Segment()
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
                    Tatums = f.Tatums.Select(b => new SpotifyModels.TimeInterval()
                    {
                        Start = b.Start,
                        Duration = b.Duration,
                        Confidence = b.Confidence
                    }).ToList()
                }).FirstOrDefault();
            });

            var dataFolder = ConfigurationManager.AppSettings["Data"];
            Directory.CreateDirectory(@$"{dataFolder}\Artist");
            Directory.CreateDirectory(@$"{dataFolder}\Album");
            Directory.CreateDirectory(@$"{dataFolder}\Track");



            artists.ForEach(a =>
            {
                //Directory.CreateDirectory(dataFolder)
                File.WriteAllText(@$"C:\Users\jmgre\source\repos\spotify.whisperer.v1\Spotify\Data\Artist\artist-{a.Id}.json", JsonConvert.SerializeObject(a));
            });
            albums.ForEach(a =>
            {
                File.WriteAllText(@$"C:\Users\jmgre\source\repos\spotify.whisperer.v1\Spotify\Data\Album\album-{a.Id}.json", JsonConvert.SerializeObject(a));
            });
            tracks.ForEach(t =>
            {
                File.WriteAllText(@$"C:\Users\jmgre\source\repos\spotify.whisperer.v1\Spotify\Data\Track\track-{t.Id}.json", JsonConvert.SerializeObject(t));
            });

            //postgres.LoadArtists(artists);
            //postgres.LoadArtistGenres(artists);
            //postgres.LoadAlbums(albums);
            //postgres.LoadAlbumArtists(albums);
            //postgres.LoadTracks(tracks);
            //postgres.LoadTrackAlbum(albums);
            //postgres.LoadTrackArtists(tracks);




        }

        private static List<SpotifyModels.Track> MapTrackFeatures(List<SpotifyModels.Track> tracks, List<TrackAudioFeatures> audioFeatures, List<TrackAudioAnalysis> audioAnalysis)
        {
            

            return fullTracks;
        }
        private static (List<SpotifyModels.Album> albums, List<SpotifyModels.Track> tracks) GetAlbumTrackData(Spotify spotify, List<FullTrack> fullTracks, List<string> artistIds)
        {

            List<SimpleAlbum> artistSimpleAlbums = spotify.GetArtistAlbums(artistIds); //missing singles

            List<FullAlbum> artistFullAlbums = spotify.GetFullAlbums(artistSimpleAlbums.Select(x => x.Id).Distinct().ToList());

            List<Album> albums = artistFullAlbums.Select(album =>
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
                Images = album.Images,
                artistIds = album.Artists.Select(a => a.Id).ToList(),
                trackIds = album.Tracks.Items.Select(a => a.Id).ToList(),
                Genres = album.Genres.ToList()
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
                PreviewUrl = t.PreviewUrl,
                DurationMs = t.DurationMs,
                artistIds = t.Artists.Select(a => a.Id).ToList(),
                albumId = t.Album.Id
            }).ToList();


            tracks.AddRange(artistFullAlbums.SelectMany(a => a.Tracks.Items).ToList().Where(a => !tracks.Exists(t => t.Id == a.Id)).Select(track => new SpotifyModels.Track()
            {
                Id = track.Id,
                Name = track.Name,
                TrackNumber = track.TrackNumber,
                DiscNumber = track.DiscNumber,
                Explicit = track.Explicit,
                PreviewUrl = track.PreviewUrl,
                DurationMs = track.DurationMs,
                artistIds = track.Artists.Select(a => a.Id).ToList(),
                albumId = artistFullAlbums.Where(a => a.Tracks.Items.Exists(t => t.Id == track.Id)).Select(a => a.Id).FirstOrDefault()
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
                Followers = artist.Followers.Total,
                Images = artist.Images,
                Popularity = artist.Popularity,
                albums = fullTracks.Where(ft => ft.Album.Artists.Exists(a => a.Id == artist.Id)).Select(ft => ft.Album.Id).ToList(),
                tracks = fullTracks.Where(ft => ft.Artists.Exists(a => a.Id == artist.Id)).Select(ft => ft.Id).ToList(),
                Genres= artist.Genres.ToList()
                //Href = artist.Href
            }).Distinct().ToList();

            return artists;
        }
    }
}
