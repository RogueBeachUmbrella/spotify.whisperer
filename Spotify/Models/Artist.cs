using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.SpotifyModels
{
    public class Artist
    {
        public string ExternalUrl { get; set; } = default!;
        public int Followers { get; set; } = default!;
        public List<string> Genres { get; set; } = default!;
        public string Href { get; set; } = default!;
        public string Id { get; set; } = default!;
        public List<Image> Images { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Popularity { get; set; }
        public string Type { get; set; } = default!;
        public string Uri { get; set; } = default!;
        public List<string> albumIds { get; set; }
        public List<string> trackIds { get; set; }
        public List<Album> albums { get; set; }
    }
}
