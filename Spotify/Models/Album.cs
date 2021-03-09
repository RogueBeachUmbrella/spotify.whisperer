using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SpotifyAPI.Web;

namespace Spotify.SpotifyModels
{
    public class Album
    {
        [JsonProperty("album_type")]
        public string AlbumType { get; set; } = default!;

        [JsonProperty("spotify_url")]
        public string ExternalUrl { get; set; } = default!;

        [JsonProperty("genres")]
        public List<string> Genres { get; set; } = default!;

        [JsonProperty("_id")]
        public string Id { get; set; } = default!;

        [JsonProperty("images")]
        public List<Image> Images { get; set; } = default!;

        [JsonProperty("name")]
        public string Name { get; set; } = default!;

        [JsonProperty("popularity")]
        public int Popularity { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; } = default!;

        [JsonProperty("release_date_precision")]
        public string ReleaseDatePrecision { get; set; } = default!;

        [JsonProperty("tracks")]
        public List<string> trackIds { get; set; }

        [JsonProperty("artists")]
        public List<string> artistIds { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = default!;

        [JsonProperty("uri")]
        public string Uri { get; set; } = default!;
    }
}
