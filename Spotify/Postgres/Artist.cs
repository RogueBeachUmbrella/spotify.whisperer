using Spotify;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Postgres
{
    public class Artist
	{
		public int artist_id { get; set; }
		public DateTime? created_at { get; set; }
		public DateTime? modified_at { get; set; }
		public string id { get; set; }
		public string name { get; set; }
		public int? popularity { get; set; }
		public string type { get; set; }
		public string uri { get; set; }
		public int followers { get; set; }
		public string spotify_url { get; set; }
		public List<ArtistGenre> genres { get; set; }
		public List<ArtistImage> images { get; set; }
		public List<RelatedArtist> relatedArtists { get; set; }
		//public List<Album> albums { get; set; }
		public List<ChartTrack> chartTracks { get; set; }

		//public Artist()
  //      {
		//	genres = new List<ArtistGenre>();
		//	images = new List<ArtistImage>();
		//	relatedArtists = new List<RelatedArtist>();
		//	//albums = new List<Album>();
		//	chartTracks = new List<ChartTrack>();
  //      }
	}
}
