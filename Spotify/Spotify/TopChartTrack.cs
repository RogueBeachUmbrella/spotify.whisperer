using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;

namespace Spotify
{
    public class TopChartTrack
    {

        [Name("Position")]
        public int position { get; set; }

        [Name("Track Name")]
        public string trackName { get; set; }

        [Name("Artist")]
        public string artist { get; set; }

        [Name("Streams")]
        public int streams { get; set; }

        [Name("URL")]
        public string url { get; set; }

        public string id { get { return url.Replace(@"https://open.spotify.com/track/", ""); } }
        //public List<string> spotifyArtistIds { get; set; }
        private string country { get; set; }
        private DateTime week_start { get; set; }
        private DateTime week_end { get; set; }
        public TopChartTrack()
        {
            //spotifyArtistIds = new List<string>();

            //static void SetReportParams(string countryCode, DateTime weekStart, DateTime weekEnd)
            //{
            //    country
            //}
        }
    }
}
