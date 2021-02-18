using System;

namespace Spotify
{
    public class TrackFeatures
	{
		public int track_feature_id { get; set; }
		public DateTime? created_at { get; set; }
		public DateTime? modified_at { get; set; }
		public int track_id { get; set; }
		public decimal danceability { get; set; }
		public decimal energy { get; set; }
		public int key { get; set; }
		public decimal loudness { get; set; }
		public int mode { get; set; }
		public decimal speechiness { get; set; }
		public decimal acousticness { get; set; }
		public decimal instrumentalness { get; set; }
		public decimal liveness { get; set; }
		public decimal valence { get; set; }
		public decimal tempo { get; set; }
		public int duration_ms { get; set; }
		public int time_signature { get; set; }			
	}	
}
