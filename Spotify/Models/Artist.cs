using Newtonsoft.Json;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.SpotifyModels
{
    public class Artist
    {
        [JsonProperty("spotify_url")]
        public string ExternalUrl { get; set; } = default!;

        [JsonProperty("followers")]
        public int Followers { get; set; } = default!;

        [JsonProperty("genres")]
        public List<string> Genres { get; set; } = default!;

        [JsonProperty("_id")]
        public string Id { get; set; } = default!;

        [JsonProperty("images")]
        public List<Image> Images { get; set; } = default!;

        [JsonProperty("name")]
        public string Name { get; set; } = default!;

        [JsonProperty("popularity")]
        public int? Popularity { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = default!;

        [JsonProperty("uri")]
        public string Uri { get; set; } = default!;

        [JsonProperty("albums")]
        public List<string> albums { get; set; }

        [JsonProperty("tracks")]
        public List<string> tracks { get; set; }
    }
}
