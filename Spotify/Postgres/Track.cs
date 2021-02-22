using System;
using System.Collections.Generic;

namespace Spotify
{
    public class Track
	{
		public int track_id { get; set; }
		public DateTime? created_at { get; set; }
		public DateTime? modified_at { get; set; }
		public int duration_ms { get; set; }
		public bool isexplicit { get; set; }
		public string name { get; set; }
		public int popularity { get; set; }
		public string preview_url { get; set; }
		public int track_number { get; set; }
		public string type { get; set; }
		public string uri { get; set; }
		public string spotify_url { get; set; }
		public string id { get; set; }
		public int disc_number { get; set; }
		public TrackFeatures features { get; set; }
		public AudioAnalysis audioAnalysis { get; set; }

	}

	
}
