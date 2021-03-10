using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Spotify
{
    public class SpotifyReport
    {

        public string country { get; set; }
        public DateTime week_start { get; set; }
        public DateTime week_end { get; set; }
        public List<ReportTrack> tracks { get; set; }
    }
    public class ReportTrack
    {

        [Name("Position")]
        [JsonProperty("position")]
        public int position { get; set; }

        [Name("Track Name")]
        [JsonProperty("track_name")]
        public string trackName { get; set; }

        [Name("Artist")]
        [JsonProperty("artist")]
        public string artist { get; set; }

        [Name("Streams")]
        [JsonProperty("streams")]
        public int streams { get; set; }

        [Name("URL")]
        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("track_id")]
        public string id { get { return url.Replace(@"https://open.spotify.com/track/", ""); } }
    }
}
