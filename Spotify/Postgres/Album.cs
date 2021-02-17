using System;
using System.Collections.Generic;

namespace Spotify
{
    public class Album
	{
		public int album_id { get; set; }
		public DateTime? created_at { get; set; }
		public DateTime? modified_at { get; set; }
		public string id { get; set; }
		public string album_group { get; set; }
		public string album_type { get; set; }
		public string name { get; set; }
		public string release_date { get; set; }
		public string release_date_precision { get; set; }
		public int total_tracks { get; set; }
		public string type { get; set; }
		public string uri { get; set; }
		public string spotify_url { get; set; }
		public List<Track> tracks { get; set; }

		public Album()
        {
			tracks = new List<Track>();
        }
	}
}
