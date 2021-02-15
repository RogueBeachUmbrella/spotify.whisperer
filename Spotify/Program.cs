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
            var spotify = new Spotify();
            var postgres = new Postgres();

            var chartTracks = spotify.GetChartTracks();
            var tracks = spotify.GetTracks(chartTracks.Select(ct => ct.id).ToList());
            var artists = spotify.GetTrackArtists(tracks);


            var relatedArtists = spotify.GetArtistRelatedArtists(artists);
            var artistAlbums = spotify.GetArtistAlbums(artists);
            var albumTracks = spotify.GetAlbumTracks(artistAlbums);

            spotify.JoinArtistData(artists, relatedArtists, artistAlbums, albumTracks);


            //var pgTracks = PGLoadTracks(tracks);
            //var pgArtists = PGLoadArtists(artists);
            //var pgChartTracks = PGLoadChartTracks(chartTracks, pgArtists, pgTracks, tracks);



            var trackAudioFeatures = spotify.GetTrackAudioFeatures(chartTracks);
            var trackAudioAnalysis = spotify.GetTrackAudioAnalysis(chartTracks);

        }
    }
}
