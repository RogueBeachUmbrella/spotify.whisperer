using System;

namespace Spotify
{
    public class Pitch
	{
		public int track_segment_pitch_id { get; set; }
		public DateTime? created_at { get; set; }
		public DateTime? modified_at { get; set; }
		public int track_segment_id { get; set; }
		public decimal pitch { get; set; }
	}

	
}
