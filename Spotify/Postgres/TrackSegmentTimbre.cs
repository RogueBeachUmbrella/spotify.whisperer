using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify
{

    public class TrackSegmentTimbre
	{
		public int track_segment_timbre_id { get; set; }
		public DateTime? created_at { get; set; }
		public DateTime? modified_at { get; set; }
		public int track_segment_id { get; set; }
		public decimal timbre { get; set; }
	}
}
