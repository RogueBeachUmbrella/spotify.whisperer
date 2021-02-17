using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;

namespace Spotify
{
    public class ChartReportTrack
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

        public string spotifyTrackId { get { return url.Replace(@"https://open.spotify.com/track/", ""); } }
        public List<string> spotifyArtistIds { get; set; }

        public ChartReportTrack()
        {
            spotifyArtistIds = new List<string>();
        }
    }
}
