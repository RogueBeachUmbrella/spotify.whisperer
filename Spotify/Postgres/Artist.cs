using System;
using System.Collections.Generic;

namespace Spotify
{
    public class Artist
	{
		public int artist_id { get; set; }
		public DateTime? created_at { get; set; }
		public DateTime? modified_at { get; set; }
		public string id { get; set; }
		public string name { get; set; }
		public int popularity { get; set; }
		public string type { get; set; }
		public string uri { get; set; }
		public int followers { get; set; }
		public string spotify_url { get; set; }
		public List<ArtistGenre> genres { get; set; }
		public List<ArtistImage> images { get; set; }
		public List<RelatedArtist> relatedArtists { get; set; }
		public List<Album> albums { get; set; }
	}
}
