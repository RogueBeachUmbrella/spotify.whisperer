﻿using System;

namespace Spotify
{
    public class ChartTrack
	{
		public int chart_track_id { get; set; }
		public DateTime? created_at { get; set; }
		public DateTime? modified_at { get; set; }
		public int artist_id { get; set; }
		public int track_id { get; set; }
		public int position { get; set; }
		public int streams { get; set; }
		public string country { get; set; }
		public DateTime week_start { get; set; }
		public DateTime week_end { get; set; }
	}	
}
