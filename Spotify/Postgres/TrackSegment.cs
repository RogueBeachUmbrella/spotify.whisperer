using System;
using System.Collections.Generic;

namespace Spotify
{
    public class TrackSegment
	{
		public int track_segment_id { get; set; }
		public DateTime? created_at { get; set; }
		public DateTime? modified_at { get; set; }
		public int track_id { get; set; }
		public decimal start { get; set; }
		public decimal duration { get; set; }
		public decimal confidence { get; set; }
		public decimal loudness_start { get; set; }
		public decimal loudness_max_time { get; set; }
		public decimal loudness_max { get; set; }
		public decimal loudness_end { get; set; }
		public List<Pitch> pitches { get; set; }
		public List<Timbre> timbres { get; set; }
		public TrackSegment()
		{
			pitches = new List<Pitch>();
			timbres = new List<Timbre>();
		}
	}
}
