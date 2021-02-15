using System;
using System.Collections.Generic;

namespace Spotify
{
    public class TrackSection
	{
		public int track_section_id { get; set; }
		public DateTime? created_at { get; set; }
		public DateTime? modified_at { get; set; }
		public int track_id { get; set; }
		public decimal start { get; set; }
		public decimal duration { get; set; }
		public decimal confidence { get; set; }
		public decimal loudness { get; set; }
		public decimal tempo { get; set; }
		public decimal tempo_confidence { get; set; }
		public int key { get; set; }
		public decimal key_confidence { get; set; }
		public int mode { get; set; }
		public decimal mode_confidence { get; set; }
		public int time_signature { get; set; }
		public decimal time_signature_confidence { get; set; }
		public List<TrackSegmentPitch> pitches { get; set; }
		public List<TrackSegmentTimbre> timbres { get; set; }
	}

	
}
