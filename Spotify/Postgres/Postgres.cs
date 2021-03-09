using Npgsql;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Data;
using Dapper;
using System;
using Postgres;

namespace Spotify

{
    public class Postgres
    {
        private NpgsqlConnection _connection { get; set; }
        public Postgres()
        {
            _connection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString);
        }

        public void LoadArtists(List<SpotifyModels.Artist> artists)
        {
            string insert;
            insert = "INSERT INTO artist(id, name, popularity, type, uri, followers, spotify_url) " +
                     "VALUES (@id, @name, @popularity, @type, @uri, @followers, @spotify_url) " +
                     "ON CONFLICT DO NOTHING;";
            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                var rowsAffected = db.Execute(insert, artists.Select(a => new Artist()
                {
                    id = a.Id,
                    name = a.Name,
                    popularity = a.Popularity,
                    type = a.Type,
                    uri = a.Uri,
                    followers = a.Followers,
                    spotify_url = a.ExternalUrl
                }).ToList());
            }  
        }
        public void LoadArtistGenres(List<SpotifyModels.Artist> artists)
        {
            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                //genre enum
                artists.Where(a => a.Genres != null)
                .SelectMany(a => a?.Genres)
                .Distinct()
                .ToList()
                .ForEach(g =>
                {
                    try
                    {
                        db.Execute(@$"ALTER TYPE genre ADD VALUE '{g}'");
                    }
                    catch (Exception e)
                    {

                    }
                    //try
                    //{
                    //    db.Execute(@$"INSERT INTO genres(genre) VALUES ('{g}') ON CONFLICT DO NOTHING;");
                    //}
                    //catch (Exception e)
                    //{

                    //}
                });

                //artist genre
                var values = new List<KeyValuePair<string, string>>();
                artists.Where(a => a.Genres != null)
                    .ToList()
                    .ForEach(a => a.albums
                        .ForEach(b => values.Add(new KeyValuePair<string, string>(a.Id, b)
                        )
                    )
                );
                var insert = @$"INSERT INTO artist_genre(artistId, genre) VALUES (@artistId, @genre) ON CONFLICT DO NOTHING;";
                var rowsAffected = db.Execute(insert, values.Select(x => new { artistId = x.Key, albumId = x.Value }));
            }
        }
        public void LoadAlbums(List<SpotifyModels.Album> albums)
        {
            string insert;
            insert = "INSERT INTO album(id, album_group, album_type, name, release_date, release_date_precision, total_tracks, type, uri, spotify_url) " +
                     "VALUES (@id, @album_group, @album_type, @name, @release_date, @release_date_precision, @total_tracks, @type, @uri, @spotify_url)" +
                     "ON CONFLICT DO NOTHING;";
            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                var rowsAffected = db.Execute(insert, albums.Select(a => new Album()
                {
                    id = a.Id,
                    name = a.Name,                    
                    type = a.Type,
                    uri = a.Uri,
                    spotify_url = a.ExternalUrl,
                    album_group = a.AlbumType,
                    album_type = a.AlbumType,
                    release_date = a.ReleaseDate,
                    release_date_precision = a.ReleaseDatePrecision
                    //total_tracks = a.trackIds.Count(),                 
                }).ToList());
            }
        }
        public void LoadAlbumArtists(List<SpotifyModels.Album> albums)
        {
            var values = new List<KeyValuePair<string, string>>();
            albums.Where(album => album.artistIds != null).ToList().ForEach(album => album.artistIds.ForEach(artistId => values.Add(new KeyValuePair<string, string>(album.Id, artistId))));

            string insert;
            insert = "INSERT INTO album_artist(artistId, albumId) VALUES (@artistId, @albumId) ON CONFLICT DO NOTHING;";
            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                var rowsAffected = db.Execute(insert, values.Where(a => a.Value != null).ToList().Select(x => new { albumId = x.Key, artistId = x.Value}));
            }
        }
        public class TrackMap 
        {
            public int track_id { get; set; }
            public string Id { get; set; }
            public SpotifyModels.Track track { get; set; }
        }
        public List<TrackMap> GetTrackIdMap(List<SpotifyModels.Track> tracks)
        {
            var sql = "SELECT track_id, id FROM track where id = @Id;";
            var trackMap = new List<TrackMap>();
            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                tracks.Select(track => new { track = track, Id = track.Id })
                    .ToList()
                    .ForEach(t => {
                        var query = db.Query<TrackMap>(sql, new { Id = t.Id })
                        .Select(q => new TrackMap() { 
                            track = t.track,
                            track_id = q.track_id,
                            Id = q.Id

                        });
                        trackMap.AddRange(query);
                    });


                //.Select(m =>
                //new TrackMap()
                //{
                //    track = tracks.Where(t => t.Id == m.trackId).FirstOrDefault(),
                //    trackId = m.trackId,
                //    track_id = m.track_id
                //}).ToList();

            


                return trackMap;
            }
        }
        public void LoadTracks(List<SpotifyModels.Track> tracks)
        {
            string insert;
            insert = "INSERT INTO track(duration_ms, isexplicit, name, popularity, preview_url, track_number, type, uri, spotify_url, id) " +
                     "VALUES (@duration_ms, @isexplicit, @name, @popularity, @preview_url, @track_number, @type, @uri, @spotify_url, @id)" +
                     "ON CONFLICT DO NOTHING;";
            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                var rowsAffected = db.Execute(insert, tracks.Select(t => new Track()
                {
                    disc_number = t.DiscNumber,
                    duration_ms = t.DurationMs,
                    isexplicit = t.Explicit,
                    name = t.Name,
                    popularity = t.Popularity,
                    preview_url = t.PreviewUrl,
                    track_number = t.TrackNumber,
                    type = "track",
                    uri = t.ExternalUrl,
                    spotify_url = t.ExternalUrl,
                    id = t.Id
                    //total_tracks = a.trackIds.Count(),                 
                }).ToList());
            }

            var trackIdMap = GetTrackIdMap(tracks).Join(tracks, m => m.Id, t => t.Id, (m, t) => new TrackMap() { track_id = m.track_id, Id = t.Id, track = t }).ToList();

            LoadTrackAudioAnalisys(trackIdMap);
            LoadTrackBars(trackIdMap);
            LoadTrackBeats(trackIdMap);
            LoadTrackSection(trackIdMap);
            LoadTrackSegmentPitch(trackIdMap);
            LoadTrackSegmentTimbre(trackIdMap);
            LoadTrackSegments(trackIdMap);
            LoadTrackTatums(trackIdMap);
        }
        private static void LoadTrackTatums(List<TrackMap> trackIdMap)
        {
            string insert = @$"INSERT INTO track_tatum(track_id, start, duration, confidence) 
                               VALUES(@track_id, @start, @duration, @confidence) 
                               ON CONFLICT DO NOTHING;";

            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                var values = trackIdMap.Where(t => t.track.audioAnalysis != null).SelectMany(t => t.track.audioAnalysis.Tatums.ToList().Select(b =>
                new TrackTatum()
                {
                    track_id = t.track_id,
                    start = (decimal)b.Start,
                    duration = (decimal)b.Duration,
                    confidence = (decimal)b.Confidence
                })).ToList();
                var rowsAffected = db.Execute(insert, values);
            }
        }
        private static void LoadTrackSegments(List<TrackMap> trackIdMap)
        {
           
            string insert = @$"INSERT INTO track_segment(track_id, start, duration, confidence, loudness_start, loudness_max_time, loudness_max, loudness_end) 
                               VALUES (@track_id, @start, @duration, @confidence, @loudness_start, @loudness_max_time, @loudness_max, @loudness_end)
                               ON CONFLICT DO NOTHING;";

            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                var values = trackIdMap.Where(t => t.track.audioAnalysis != null).SelectMany(t => t.track.audioAnalysis.Segments.Select(s =>
                new TrackSegment()
                {
                    track_id = t.track_id,
                    start = (decimal)s.Start,
                    duration = (decimal)s.Duration,
                    confidence = (decimal)s.Confidence,
                    loudness_start = (decimal)s.LoudnessStart,
                    loudness_max_time = (decimal)s.LoudnessMaxTime,
                    loudness_max = (decimal)s.LoudnessMax,
                    loudness_end = (decimal)s.LoudnessEnd
                })).ToList();
                var rowsAffected = db.Execute(insert, values);
            }
        }
        private static void LoadTrackSegmentPitch(List<TrackMap> trackIdMap)
        {
            string insert = @$"INSERT INTO track_segment_pitch(trackId, start, duration, pitch) 
                            VALUES (@trackId, @start, @duration, @pitch) 
                            ON CONFLICT DO NOTHING;";

            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                var values = trackIdMap.Where(t => t.track.audioAnalysis != null).SelectMany(t => t.track.audioAnalysis.Segments.SelectMany(s => s.Pitches.Select(p => 
                new 
                { 
                    trackId = t.Id,
                    start = s.Start,
                    duration = s.Duration,
                    pitch = p 
                }))).ToList();
                var rowsAffected = db.Execute(insert, values);
            }
        }
        private static void LoadTrackSegmentTimbre(List<TrackMap> trackIdMap)
        {
            string insert = @$"INSERT INTO track_segment_timbre(trackId, start, duration, timbre) 
                            VALUES (@trackId, @start, @duration, @timbre) 
                            ON CONFLICT DO NOTHING;";

            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                var values = trackIdMap.Where(t => t.track.audioAnalysis != null).SelectMany(t => t.track.audioAnalysis.Segments.SelectMany(s => s.Timbre.Select(p =>
                new
                {
                    trackId = t.Id,
                    start = s.Start,
                    duration = s.Duration,
                    timbre = p
                }))).ToList();
                var rowsAffected = db.Execute(insert, values);
            }
        }
        private static void LoadTrackSection(List<TrackMap> trackIdMap)
        {
            string insert = @$"INSERT INTO track_section(track_id, start, duration, confidence, loudness, tempo, tempo_confidence, key, key_confidence, mode, mode_confidence, time_signature, time_signature_confidence) 
                               VALUES (@track_id, @start, @duration, @confidence, @loudness, @tempo, @tempo_confidence, @key, @key_confidence, @mode, @mode_confidence, @time_signature, @time_signature_confidence) 
                               ON CONFLICT DO NOTHING;";

            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                var values = trackIdMap.Where(t => t.track.audioAnalysis != null).SelectMany(t => t.track.audioAnalysis.Sections.Select(s =>
                new TrackSection()
                {
                    track_id = t.track_id,
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
                })).ToList();
                var rowsAffected = db.Execute(insert, values);
            }
        }
        private static void LoadTrackBeats(List<TrackMap> trackIdMap)
        {
            string insert = @$"INSERT INTO track_beat(track_id, start, duration, confidence) 
                               VALUES(@track_id, @start, @duration, @confidence) 
                               ON CONFLICT DO NOTHING;";

            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                var values = trackIdMap.Where(t => t.track.audioAnalysis != null).SelectMany(t => t.track.audioAnalysis.Beats.Select(b =>
                new TrackBeat()
                {
                    track_id = t.track_id,
                    start = (decimal)b.Start,
                    duration = (decimal)b.Duration,
                    confidence = (decimal)b.Confidence
                })).ToList();
                var rowsAffected = db.Execute(insert, values);
            }
        }
        private static void LoadTrackBars(List<TrackMap> trackIdMap)
        {
            string insert = @$"INSERT INTO track_bar(track_id, start, duration, confidence) 
                               VALUES(@track_id, @start, @duration, @confidence) 
                               ON CONFLICT DO NOTHING;";

            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                var values = trackIdMap.Where(t => t.track.audioAnalysis != null).SelectMany(t => t.track.audioAnalysis.Bars.Select(b =>
                new TrackBar()
                {
                    track_id = t.track_id,
                    start = (decimal)b.Start,
                    duration = (decimal)b.Duration,
                    confidence = (decimal)b.Confidence
                }).ToList()).ToList();
                var rowsAffected = db.Execute(insert, values);
            }
        }
        private static void LoadTrackAudioAnalisys(List<TrackMap> trackIdMap)
        {
            string insert = @$"INSERT INTO track_audio_analysis(track_id, num_samples, duration, sample_md5, offset_seconds, window_seconds, analysis_sample_rate, analysis_channels, end_of_fade_in, start_of_fade_out, loudness, tempo, tempo_confidence, time_signature, time_signature_confidence, key, key_confidence, mode, mode_confidence) 
                               VALUES (@track_id, @num_samples, @duration, @sample_md5, @offset_seconds, @window_seconds, @analysis_sample_rate, @analysis_channels, @end_of_fade_in, @start_of_fade_out, @loudness, @tempo, @tempo_confidence, @time_signature, @time_signature_confidence, @key, @key_confidence, @mode, @mode_confidence)
                               ON CONFLICT DO NOTHING;";

            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                //check for nulls
                var rowsAffected = db.Execute(insert, trackIdMap.Where(t => t.track.audioAnalysis != null).Select(t => new AudioAnalysis()
                {
                    num_samples = t.track.audioAnalysis.Analysis.num_samples,
                    duration = (decimal)t.track.audioAnalysis.Analysis.duration,
                    sample_md5 = t.track.audioAnalysis.Analysis.sample_md5,
                    offset_seconds = t.track.audioAnalysis.Analysis.offset_seconds,
                    window_seconds = t.track.audioAnalysis.Analysis.window_seconds,
                    analysis_sample_rate = t.track.audioAnalysis.Analysis.analysis_sample_rate,
                    analysis_channels = t.track.audioAnalysis.Analysis.analysis_channels,
                    end_of_fade_in = (decimal)t.track.audioAnalysis.Analysis.end_of_fade_in,
                    start_of_fade_out = (decimal)t.track.audioAnalysis.Analysis.start_of_fade_out,
                    loudness = (decimal)t.track.audioAnalysis.Analysis.loudness,
                    tempo = (decimal)t.track.audioAnalysis.Analysis.tempo,
                    tempo_confidence = (decimal)t.track.audioAnalysis.Analysis.tempo_confidence,
                    time_signature = t.track.audioAnalysis.Analysis.time_signature,
                    time_signature_confidence = (decimal)t.track.audioAnalysis.Analysis.time_signature_confidence,
                    key = t.track.audioAnalysis.Analysis.key,
                    key_confidence = (decimal)t.track.audioAnalysis.Analysis.key_confidence,
                    mode = t.track.audioAnalysis.Analysis.mode,
                    mode_confidence = (decimal)t.track.audioAnalysis.Analysis.mode_confidence
                }).ToList());
            }
        }
        public void LoadTrackAlbum(List<SpotifyModels.Album> albums)
        {
            var values = new List<KeyValuePair<string, string>>();
            albums.ForEach(album => album.trackIds.ForEach(trackId => values.Add(new KeyValuePair<string, string>(album.Id, trackId))));

            string insert;
            insert = "INSERT INTO track_album(trackId, albumId) VALUES (@trackId, @albumId) ON CONFLICT DO NOTHING;";
            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                var rowsAffected = db.Execute(insert, values.Select(x => new { albumId = x.Key, trackId = x.Value }));
            }
        }
        public void LoadTrackArtists(List<SpotifyModels.Track> tracks)
        {
            var values = new List<KeyValuePair<string, string>>();
            tracks.Where(t => t.artistIds != null).ToList().ForEach(track => track.artistIds.ForEach(artistId => values.Add(new KeyValuePair<string, string>(track.Id, artistId))));

            string insert;
            insert = "INSERT INTO track_artist(artistId, trackId) VALUES (@artistId, @trackId) ON CONFLICT DO NOTHING;";
            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                var rowsAffected = db.Execute(insert, values.Select(x => new { trackId = x.Key, artistId = x.Value }));
            }
        }




    

        //                track.audioAnalysis.sections.ForEach(section => {
        //                    //track section
        //                    getId = @$"select track_section_id from track_section where track_id = @track_id and start = @start and duration = @duration and confidence = @confidence;";
        //                    insert = @$"INSERT INTO track_section(track_id, start, duration, confidence, loudness, tempo, tempo_confidence, key, key_confidence, mode, mode_confidence, time_signature, time_signature_confidence) VALUES (@track_id, @start, @duration, @confidence, @loudness, @tempo, @tempo_confidence, @key, @key_confidence, @mode, @mode_confidence, @time_signature, @time_signature_confidence);";
        //                    section.track_id = track.track_id;
        //                    if (db.QuerySingleOrDefault<int>(getId, section) == 0)
        //                    {
        //                        db.QuerySingleOrDefault<int>(insert, section);
        //                    }
        //                });

        //                track.audioAnalysis.segments.ForEach(segment => {
        //                    //track segment
        //                    getId = @$"select track_segment_id from track_segment where track_id = @track_id and start = @start and duration = @duration and confidence = @confidence;";
        //                    insert = @$"INSERT INTO track_segment(track_id, start, duration, confidence, loudness_start, loudness_max_time, loudness_max, loudness_end) VALUES (@track_id, @start, @duration, @confidence, @loudness_start, @loudness_max_time, @loudness_max, @loudness_end) RETURNING track_segment_id;";
        //                    segment.track_id = track.track_id;
        //                    segment.track_segment_id = db.QuerySingleOrDefault<int>(getId, segment);
        //                    if (segment.track_segment_id == 0)
        //                    {
        //                        segment.track_segment_id = db.QuerySingle<int>(insert, segment);
        //                    }

        //                    segment.pitches.ForEach(pitch => {
        //                        //track segment pitch
        //                        getId = @$"select track_segment_pitch_id from track_segment_pitch where track_segment_id = @track_segment_id and pitch = @pitch;";
        //                        insert = @$"INSERT INTO track_segment_pitch(track_segment_id, pitch) VALUES (@track_segment_id, @pitch);";
        //                        pitch.track_segment_id = segment.track_segment_id;
        //                        if (db.QuerySingleOrDefault<int>(getId, pitch) == 0)
        //                        {
        //                            db.QuerySingleOrDefault<int>(insert, pitch);
        //                        }
        //                    });

        //                    segment.timbres.ForEach(timbre => {
        //                        //track segment timbre
        //                        getId = @$"select track_segment_timbre_id from track_segment_timbre where track_segment_id = @track_segment_id and timbre = @timbre;";
        //                        insert = @$"INSERT INTO track_segment_timbre(track_segment_id, timbre) VALUES (@track_segment_id, @timbre);";
        //                        timbre.track_segment_id = segment.track_segment_id;
        //                        if (db.QuerySingleOrDefault<int>(getId, new { segment.track_segment_id, timbre.timbre }) == 0)
        //                        {
        //                            db.QuerySingleOrDefault<int>(insert, new { segment.track_segment_id, timbre.timbre });
        //                        }
        //                    });
        //                    artist.chartTracks.ForEach(chartTrack => {

        //                        chartTrack.track_id = track.track_id;
        //                        chartTrack.artist_id = artist.artist_id;

        //                        getId = @$"SELECT chart_track_id FROM chart_track WHERE artist_id = @artist_id, track_id = @track_id, country = @country, week_start = @week_start, week_end = @week_end;";
        //                        insert = $"INSERT INTO chart_track(artist_id, track_id, \"position\", streams, country, week_start, week_end) VALUES (@artist_id, @track_id, @position, @streams, @country, @week_start, @week_end);";

        //                        try {             
        //                            if (db.QuerySingleOrDefault<int>(getId, chartTrack) == 0)
        //                            {
        //                                db.QuerySingleOrDefault<int>(insert, chartTrack);
        //                            }
        //                        }
        //                        catch(Exception ex)
        //                        {

        //                        }
        //                    });
        //                });//end track segment loop
        //            });// end track loop                       
        //        });// end album loop
        //    }
        //}
































        //static void PGLoadRelatedArtists(Dictionary<string, List<FullArtist>> relatedArtists, List<Artist> pgArtists)
        //{
        //    string sqlQuery = "INSERT INTO related_artist(artist_id, related_id) VALUES (@artist_id, @related_id);";
        //    relatedArtists.ToList().ForEach(ra => {
        //        int artistId = pgArtists.Where(pga => pga.id == ra.Key).Select(pga => pga.artist_id).Single();
        //        ra.Value.ForEach(ra => {
        //            var pgRelaredArtist = new RelatedArtist()
        //            {
        //                artist_id = artistId,
        //                related_artist_id = pgArtists.Where(pga => pga.id == ra.Id).Select(pga => pga.artist_id).Single()
        //            };
        //        });
        //    });
        //}
        //static List<Artist> PGLoadArtistImages(Dictionary<string, FullArtist> artists, List<Artist> pgArtists)
        //{
        //    var pgArtistImages = new List<ArtistImage>();
        //    string sqlQuery = "INSERT INTO artist_image(artist_id, height, url, width) VALUES (@artist_id, @height, @url, @width) RETURNING artist_image_id;";
        //    pgArtists.ForEach(p => {
        //        artists.Where(a => a.Key == p.id).Single().Value.Images.ForEach(i => {
        //            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
        //            {
        //                artists.ToList().ForEach(artist =>
        //                {
        //                    var pgArtistImage = new ArtistImage()
        //                    {
        //                        artist_id = p.artist_id,
        //                        height = i.Height,
        //                        url = i.Url,
        //                        width = i.Width
        //                    };
        //                    pgArtistImage.artist_image_id = db.QuerySingle<int>(sqlQuery, pgArtistImage);
        //                    pgArtistImages.Add(pgArtistImage);
        //                });
        //            }
        //        });
        //    });
        //    return pgArtists;
        //}
        //static List<Artist> PGLoadAlbums(Dictionary<string, List<SimpleAlbum>> artistAlbums, List<Artist> pgArtists)
        //{
        //    var pgAlbums = new List<Album>();
        //    string sqlQuery;
        //    artistAlbums.ToList().ForEach(artist => {
        //        artist.Value.ForEach(album =>
        //        {
        //            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
        //            {
        //                sqlQuery = "INSERT INTO album(id, album_group, album_type, name, release_date, release_date_precision, total_tracks, type, uri, spotify_url) VALUES (@id, @album_group, @album_type, @name, @release_date, @release_date_precision, @total_tracks, @type, @uri, @spotify_url) RETURNS album_id;";
        //                var pgAlbum = new Album()
        //                {
        //                    id = album.Id,
        //                    album_group = album.AlbumGroup,
        //                    album_type = album.AlbumType,
        //                    name = album.Name,
        //                    release_date = album.ReleaseDate,
        //                    release_date_precision = album.ReleaseDatePrecision,
        //                    total_tracks = 1, //TODO: get total tracks
        //                    type = album.Type,
        //                    uri = album.Uri,
        //                    spotify_url = album.ExternalUrls.FirstOrDefault().Value
        //                };
        //                pgAlbum.album_id = db.QuerySingle<int>(sqlQuery, pgAlbum);
        //                pgAlbums.Add(pgAlbum);

        //                album.Images.ForEach(i => {
        //                    sqlQuery = "INSERT INTO album_image(album_id, height, url, width) VALUES (@album_id, @height, @url, @width) RETURNING album_image_id;";
        //                    var pgAlbumImage = new AlbumImage()
        //                    {
        //                        album_id = pgAlbum.album_id,
        //                        height = i.Height,
        //                        url = i.Url,
        //                        width = i.Width
        //                    };
        //                });







        //            }
        //            var x = new Album()
        //            {

        //            };
        //        });
        //    });



        //    //    .ForEach(p => {
        //    //    artists.Where(a => a.Key == p.id).Single().Value.Images.ForEach(i => {
        //    //        using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
        //    //        {
        //    //            artists.ToList().ForEach(artist =>
        //    //            {
        //    //                var pgArtistImage = new ArtistImage()
        //    //                {
        //    //                    artist_id = p.artist_id,
        //    //                    height = i.Height,
        //    //                    url = i.Url,
        //    //                    width = i.Width
        //    //                };
        //    //                pgArtistImage.artist_image_id = db.QuerySingle<int>(sqlQuery, pgArtistImage);
        //    //                pgArtistImages.Add(pgArtistImage);
        //    //            });

        //    //        }
        //    //    });
        //    //});
        //    return pgArtists;
        //}
        //static List<ChartReportTrack> PGLoadChartTracks(List<ChartReportTrack> chartTracks, List<Artist> artists, List<Track> tracks, List<FullTrack> fullTracks)
        //{
        //    var query = from chart in chartTracks
        //                join track in tracks on chart.spotifyTrackId equals track.id
        //                join fullTrack in fullTracks on chart.spotifyTrackId equals fullTrack.Id
        //                join artist in artists on fullTrack.Artists.Where(a => a.Name == chart.artist).FirstOrDefault().Name equals artist.name
        //                select new ChartTrack()
        //                {
        //                    artist_id = artist.artist_id,
        //                    track_id = track.track_id,
        //                    position = chart.position,
        //                    streams = chart.streams,
        //                    country = "US",
        //                    week_start = new DateTime(2020, 1, 22),
        //                    week_end = new DateTime(2020, 1, 29)
        //                };

        //    var pgChartTracks = new List<ChartReportTrack>();
        //    using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
        //    {
        //        string sqlQuery = $"INSERT INTO chart_track(artist_id, track_id, \"position\", streams, country, week_start, week_end) VALUES(@artist_id, @track_id, @position, @streams, @country, @week_start, @week_end); ";
        //        query.ToList().ForEach(q => {
        //            db.Execute(sqlQuery, q);
        //        });
        //    }
        //    return pgChartTracks;
        //}
    }
}
