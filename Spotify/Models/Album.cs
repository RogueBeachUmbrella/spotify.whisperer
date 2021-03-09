using System;
using System.Collections.Generic;
using System.Text;
using SpotifyAPI.Web;

namespace Spotify.SpotifyModels
{
    public class Album
    {
        public string AlbumType { get; set; } = default!;
        public string ExternalUrl { get; set; } = default!;
        public List<string> Genres { get; set; } = default!;
        public string Href { get; set; } = default!;
        public string Id { get; set; } = default!;
        public List<Image> Images { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Popularity { get; set; }
        public string ReleaseDate { get; set; } = default!;
        public string ReleaseDatePrecision { get; set; } = default!;
        public List<string> trackIds { get; set; }
        public List<string> artistIds { get; set; }
        public List<Track> Tracks { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Uri { get; set; } = default!;
    }
}
