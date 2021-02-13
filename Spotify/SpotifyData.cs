using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify
{
    static class Spotify
    {
        public class ChartTrack
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
        }
        public class FullArtist
        {
            public int TotalFollowers { get; set; } = default!;
            public List<string> Genres { get; set; } = default!;
            public string Id { get; set; } = default!;
            public List<Image> Images { get; set; } = default!;
            public string Name { get; set; } = default!;
            public int Popularity { get; set; }
            public string Type { get; set; } = default!;
            public List<Artist> RelatedArtist { get; set; }
            public List<Album> Albums { get; set; }

        }
        public class Artist
        {
            public string Id { get; set; } = default!;
            public string Name { get; set; } = default!;

        }
        public class Image
        {
            public int Height { get; set; }
            public int Width { get; set; }
            public string Url { get; set; } = default!;
        }
        public class Album
        {
            public string AlbumGroup { get; set; } = default!;
            public string AlbumType { get; set; } = default!;
            public List<string> ArtistsIds { get; set; } = default!;
            public List<string> AvailableMarkets { get; set; } = default!;
            public string Id { get; set; } = default!;
            public List<Image> Images { get; set; } = default!;
            public string Name { get; set; } = default!;
            public string ReleaseDate { get; set; } = default!;
            public string ReleaseDatePrecision { get; set; } = default!;
            public string Type { get; set; } = default!;
        }
    }
}
