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

            
            var (chartTracks, artists, chartReport) = spotify.GetChartData(spotify.GetChartTracks());

            //artists = spotify.GetArtistRelatedArtists(artists);
            //artists = spotify.GetArtistAlbums(artists);

            //var albumTracks = new List<Track>();

            //(artists, albumTracks) = spotify.GetArtistAlbumTracks(artists);           

            //chartTracks = spotify.GetTrackAudioFeatures(chartTracks);
            //chartTracks = spotify.GetTrackAudioAnalysis(chartTracks);     

            //albumTracks = spotify.GetTrackAudioFeatures(albumTracks);
            //albumTracks = spotify.GetTrackAudioAnalysis(albumTracks);    


            //chartTracks = spotify.UpdateChartReportTrackArtistId(tracks, chartTracks);
            //var SpotifyArtists = spotify.GetTrackArtists(tracks);
            //spotify.JoinArtistData(SpotifyArtists, spotifyRelatedArtists, spotifyArtistAlbums, spotifyAlbumTracks);
            chartTracks = Postgres.LoadTracks(chartTracks);
            artists = Postgres.LoadArtists(artists);
            //var pgChartTracks = PGLoadChartTracks(chartTracks, pgArtists, pgTracks, tracks);
        }
    }
}
