using System;

namespace Spotify
{
    public class TrackBeat
	{
		public int track_beat_id { get; set; }
		public DateTime? created_at { get; set; }
		public DateTime? modified_at { get; set; }
		public int track_id { get; set; }
		public decimal start { get; set; }
		public decimal duration { get; set; }
		public decimal confidence { get; set; }
	}	
}
