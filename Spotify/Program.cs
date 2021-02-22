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

            string countryCode = "us"; 
            DateTime weekStart = new DateTime(2021, 02, 05); 
            DateTime weekEnd = new DateTime(2021, 02, 12);


            var chartReport = spotify.GetChartTracks(countryCode, weekStart, weekEnd);
            
            var (artists, chartTracks) = spotify.GetChartData(chartReport, countryCode, weekStart, weekEnd);

            


            artists.ForEach(artist => {
                spotify.RefreshToken();

                artist.GetAlbums(spotify.client);
                artist.GetAlbumTracks(spotify.client);
                artist.GetAlbumTracksAudioFeatures(spotify.client);
                artist.GetAlbumTracksAudioAnalysis(spotify.client);

                var i = 1;

                //artist.albums.ForEach(album => {
                //    album.GetTracks(spotify.client);
                //});


                //spotify.GetArtistAlbums(ref artist);
                //spotify.GetArtistAlbumTracks(ref artist);
                //spotify.GetTrackAudioFeatures(ref artist);
                //spotify.GetTrackAudioAnalysis(ref artist);
                postgres.LoadArtist(artist);            
            });

            


            //tracks.AddRange(albumTracks);
            //chartTracks = spotify.GetTrackAudioFeatures(chartTracks);
            //chartTracks = spotify.GetTrackAudioAnalysis(chartTracks);     
            //albumTracks = spotify.GetTrackAudioFeatures(albumTracks);
            //albumTracks = spotify.GetTrackAudioAnalysis(albumTracks);    
            //chartTracks = spotify.UpdateChartReportTrackArtistId(tracks, chartTracks);
            //var SpotifyArtists = spotify.GetTrackArtists(tracks);
            //spotify.JoinArtistData(SpotifyArtists, spotifyRelatedArtists, spotifyArtistAlbums, spotifyAlbumTracks);
            //tracks = Postgres.LoadTracks(tracks);
            //var pgChartTracks = PGLoadChartTracks(chartTracks, pgArtists, pgTracks, tracks);
        }
    }
}
