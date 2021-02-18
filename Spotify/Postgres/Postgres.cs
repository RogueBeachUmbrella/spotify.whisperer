using Npgsql;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Data;
using Dapper;

namespace Spotify
{
    public class Postgres
    {
        private NpgsqlConnection _connection { get; set; }
        public Postgres()
        {
            _connection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString);
        }
        public static List<Track> LoadTracks(List<Track> tracks)
        {
            string getId = @$"select track_id from track where id = @id";
            string insert = @$"INSERT INTO track(duration_ms, explicit, name, popularity, preview_url, track_number, type, uri, spotify_url, id) VALUES (@duration_ms, @isexplicit, @name, @popularity, @preview_url, @track_number, @type, @uri, @spotify_url, @id) RETURNING track_id;";
            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                tracks.ForEach(track =>
                {
                    track.track_id = db.QuerySingleOrDefault<int>(getId, track);
                    if(track.track_id == 0)
                    {
                        track.track_id = db.QuerySingle<int>(insert, track);
                    }
                });
                return tracks;
            }
        }
        public static List<Artist> LoadArtists(List<Artist> artists)
        {
            using (IDbConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                string getId;
                string insert;
                artists.ToList().ForEach(artist =>
                {
                    //artist
                    getId = "select artist_id from artist where id = @id;";
                    insert = "INSERT INTO artist(id, name, popularity, type, uri, followers, spotify_url) VALUES (@id, @name, @popularity, @type, @uri, @followers, @spotify_url) returning artist_id;";
                    artist.artist_id = db.QuerySingleOrDefault<int>(getId, artist);
                    if (artist.artist_id == 0)
                    {
                        artist.artist_id = db.QuerySingle<int>(insert, artist);
                    }

                    //artist image
                    getId = "select artist_image_id from artist_image where artist_id = @artist_id and url = @url;";
                    insert = "INSERT INTO artist_image(artist_id, height, url, width) VALUES (@artist_id, @height, @url, @width) returning artist_image_id;";
                    artist.images.ForEach(image =>
                    {
                        image.artist_id = artist.artist_id;
                        var id = db.QuerySingleOrDefault<int>(getId, image);
                        if(id == 0)
                        {
                            image.artist_image_id = db.QuerySingle<int>(insert, image);
                        }
                    });

                    //artist genre
                    var genres = db.Query<string>("select enumlabel from pg_enum;");
                    artist.genres.ForEach(genre => {
                        if (!genres.Contains(genre.genre))
                        {
                            db.Execute(@$"ALTER TYPE genre ADD VALUE '{genre.genre}'");
                        }
                        getId = @$"select artist_genre_id from artist_genre where artist_id = @artist_id and genre = '{genre.genre}';";
                        insert = @$"INSERT INTO artist_genre(artist_id, genre) VALUES (@artist_id, '{genre.genre}') RETURNING artist_genre_id;";
                        genre.artist_id = artist.artist_id;
                        genre.artist_genre_id = db.QuerySingleOrDefault<int>(getId, genre);
                        if (genre.artist_genre_id == 0)
                        {
                            genre.artist_genre_id = db.QuerySingle<int>(insert, genre);
                        }
                    });

                    artist.albums.ForEach(album => {
                        //album
                        getId = @$"select album_id from album where id = @id;";
                        insert = @$"INSERT INTO album(id, album_group, album_type, name, release_date, release_date_precision, total_tracks, type, uri, spotify_url) VALUES (@id, @album_group, @album_type, @name, @release_date, @release_date_precision, @total_tracks, @type, @uri, @spotify_url) RETURNING album_id;";
                        album.album_id = db.QuerySingleOrDefault<int>(getId, album);
                        if (album.album_id == 0)
                        {
                            album.album_id = db.QuerySingle<int>(insert, album);
                        }

                        //artist album
                        getId = @$"select album_artist_id from album_artist where artist_id = @artist_id and album_id = @album_id;";
                        insert = @$"INSERT INTO album_artist(artist_id, album_id) VALUES (@artist_id, @album_id);";
                        var id = db.QuerySingleOrDefault<int>(getId, new { album.album_id, artist.artist_id });
                        if (id == 0)
                        {
                            db.QuerySingleOrDefault<int>(insert, new { album.album_id, artist.artist_id });
                        }

                        album.tracks.ForEach(track => {
                            //Tracks
                            getId = @$"select track_id from track where id = @id";
                            insert = @$"INSERT INTO track(duration_ms, explicit, name, popularity, preview_url, track_number, type, uri, spotify_url, id)VALUES (@duration_ms, @explicit, @name, @popularity, @preview_url, @track_number, @type, @uri, @spotify_url, @id) RETURNING track_id;";
                            track.track_id = db.QuerySingleOrDefault<int>(getId, track);
                            if (track.track_id == 0)
                            {
                                track.track_id = db.QuerySingle<int>(insert, track);
                            }

                            //track album
                            getId = @$"select track_album_id from track_album where track_id = @track_id and album_id = @album_id;";
                            insert = @$"INSERT INTO track_album(track_id, album_id) VALUES (@track_id, @album_id);";
                            if (db.QuerySingleOrDefault<int>(getId, new { album.album_id, track.track_id }) == 0)
                            {
                                db.QuerySingleOrDefault<int>(insert, new { album.album_id, track.track_id, track.track_number });
                            }

                            //track artist
                            getId = @$"select track_artist_id from track_artist where track_id = @track_id and artist_id = @artist_id;";
                            insert = @$"INSERT INTO track_artist(track_id, artist_id) VALUES (@track_id, @artist_id);";
                            if (db.QuerySingleOrDefault<int>(getId, new { artist.artist_id, track.track_id }) == 0)
                            {
                                db.QuerySingleOrDefault<int>(insert, new { album.album_id, track.track_id, track.track_number });
                            }

                            //track audio analysis
                            getId = @$"select track_audio_analysis_id from track_audio_analysis where track_id = @track_id;";
                            insert = @$"INSERT INTO track_audio_analysis(track_id, num_samples, duration, sample_md5, offset_seconds, window_seconds, analysis_sample_rate, analysis_channels, end_of_fade_in, start_of_fade_out, loudness, tempo, tempo_confidence, time_signature, time_signature_confidence, key, key_confidence, mode, mode_confidence, codetext, code_version, echoprinttext, echoprint_version, synchtext, synch_version, rhythmtext, rhythm_version) VALUES (@track_id, @num_samples, @duration, @sample_md5, @offset_seconds, @window_seconds, @analysis_sample_rate, @analysis_channels, @end_of_fade_in, @start_of_fade_out, @loudness, @tempo, @tempo_confidence, @time_signature, @time_signature_confidence, @key, @key_confidence, @mode, @mode_confidence, @codetext, @code_version, @echoprinttext, @echoprint_version, @synchtext, @synch_version, @rhythmtext, @rhythm_version);";
                            if (db.QuerySingleOrDefault<int>(getId, new { artist.artist_id, track.track_id }) == 0)
                            {
                                db.QuerySingleOrDefault<int>(insert, track.audioAnalysis);
                            }

                            track.audioAnalysis.bars.ForEach(bar => {
                                //track bar
                                getId = @$"select track_bar_id from track_bar where track_id = @track_id and start = @start and duration = @duration and confidence = @confidence;";
                                insert = @$"INSERT INTO track_bar(track_id, start, duration, confidence) VALUES (@track_id, @start, @duration, @confidence);";
                                if (db.QuerySingleOrDefault<int>(getId, new { track.track_id, bar.start, bar.duration, bar.confidence }) == 0)
                                {
                                    db.QuerySingleOrDefault<int>(insert, new { track.track_id, bar.start, bar.duration, bar.confidence });
                                }
                            });

                            track.audioAnalysis.beats.ForEach(beat => {
                                //track beat
                                getId = @$"select track_beat_id from track_beat where track_id = @track_id and start = @start and duration = @duration and confidence = @confidence;";
                                insert = @$"INSERT INTO track_beat(track_id, start, duration, confidence) VALUES (@track_id, @start, @duration, @confidence);";
                                if (db.QuerySingleOrDefault<int>(getId, new { track.track_id, beat.start, beat.duration, beat.confidence }) == 0)
                                {
                                    db.QuerySingleOrDefault<int>(insert, new { track.track_id, beat.start, beat.duration, beat.confidence });
                                }
                            });

                            track.audioAnalysis.tatums.ForEach(tatum => {
                                //track tatum
                                getId = @$"select track_tatum_id from track_tatum where track_id = @track_id and start = @start and duration = @duration and confidence = @confidence;";
                                insert = @$"INSERT INTO track_tatum(track_id, start, duration, confidence) VALUES (@track_id, @start, @duration, @confidence);";
                                if (db.QuerySingleOrDefault<int>(getId, new { track.track_id, tatum.start, tatum.duration, tatum.confidence }) == 0)
                                {
                                    db.QuerySingleOrDefault<int>(insert, new { track.track_id, tatum.start, tatum.duration, tatum.confidence });
                                }
                            });

                            track.audioAnalysis.sections.ForEach(section => {
                                //track section
                                getId = @$"select track_section_id from track_section where track_id = @track_id and start = @start and duration = @duration and confidence = @confidence;";
                                insert = @$"INSERT INTO track_section(track_id, start, duration, confidence, loudness, tempo, tempo_confidence, key, key_confidence, mode, mode_confidence, time_signature, time_signature_confidence) VALUES (@track_id, @start, @duration, @confidence, @loudness, @tempo, @tempo_confidence, @key, @key_confidence, @mode, @mode_confidence, @time_signature, @time_signature_confidence);";
                                if (db.QuerySingleOrDefault<int>(getId, new { track.track_id, section }) == 0)
                                {
                                    db.QuerySingleOrDefault<int>(insert, new { track.track_id, section });
                                }
                            });

                            track.audioAnalysis.segments.ForEach(segment => {
                                //track segment
                                getId = @$"select track_segment_id from track_segment where track_id = @track_id and start = @start and duration = @duration and confidence = @confidence;";
                                insert = @$"INSERT INTO track_segment(track_id, start, duration, confidence, loudness_start, loudness_max_time, loudness_max, loudness_end) VALUES (@track_id, @start, @duration, @confidence, @loudness_start, @loudness_max_time, @loudness_max, @loudness_end) RETURNING track_segment_id;";
                                segment.track_segment_id = db.QuerySingleOrDefault<int>(getId, new { track.track_id, segment });
                                if (segment.track_segment_id == 0)
                                {
                                   segment.track_segment_id = db.QuerySingle<int>(insert, new { track.track_id, segment });
                                }

                                segment.pitches.ForEach(pitch => {
                                    //track segment pitch
                                    getId = @$"select track_segment_pitch_id from track_segment_pitch where track_segment_id = @track_segment_id and pitch = @pitch;";
                                    insert = @$"INSERT INTO track_segment_pitch(track_segment_id, pitch) VALUES (@track_segment_id, @pitch);";
                                    if (db.QuerySingleOrDefault<int>(getId, new { segment.track_segment_id, pitch.pitch }) == 0)
                                    {
                                        db.QuerySingleOrDefault<int>(insert, new { segment.track_segment_id, pitch.pitch });
                                    }
                                });

                                segment.timbres.ForEach(timbre => {
                                    //track segment timbre
                                    getId = @$"select track_segment_timbre_id from track_segment_timbre where track_segment_id = @track_segment_id and timbre = @timbre;";
                                    insert = @$"INSERT INTO track_segment_timbre(track_segment_id, timbre) VALUES (@track_segment_id, @timbre);";
                                    if (db.QuerySingleOrDefault<int>(getId, new { segment.track_segment_id, timbre.timbre }) == 0)
                                    {
                                        db.QuerySingleOrDefault<int>(insert, new { segment.track_segment_id, timbre.timbre });
                                    }
                                });
                            });//end track segment loop
                        });// end track loop                       
                    });// end album loop
                });// end artist loop
                return artists;
            }
        }


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
