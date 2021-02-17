using Npgsql;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
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
                    getId = "select COALESCE(artist_id, 0) from artist where id = @id";
                    insert = "INSERT INTO artist(id, name, popularity, type, uri, followers, spotify_url) VALUES (@id, @name, @popularity, @type, @uri, @followers, @spotify_url) returning artist_id;";
                    artist.artist_id = db.QuerySingleOrDefault<int>(getId, artist);
                    if (artist.artist_id == 0)
                    {
                        artist.artist_id = db.QuerySingle<int>(insert, artist);
                    }

                    getId = "select COALESCE(artist_image_id, 0) from artist_image where artist_id = @artist_id and url = @url";
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
                });
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
