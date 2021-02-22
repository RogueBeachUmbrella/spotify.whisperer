using System;
using System.Collections.Generic;

namespace Spotify
{
    public class AudioAnalysis
	{
		public int track_audio_analysis_id { get; set; }
		public DateTime? created_at { get; set; }
		public DateTime? modified_at { get; set; }
		public int track_id { get; set; }
		public int num_samples { get; set; }
		public decimal duration { get; set; }
		public string sample_md5 { get; set; }
		public int offset_seconds { get; set; }
		public int window_seconds { get; set; }
		public int analysis_sample_rate { get; set; }
		public int analysis_channels { get; set; }
		public decimal end_of_fade_in { get; set; }
		public decimal start_of_fade_out { get; set; }
		public decimal loudness { get; set; }
		public decimal tempo { get; set; }
		public decimal tempo_confidence { get; set; }
		public int time_signature { get; set; }
		public decimal time_signature_confidence { get; set; }
		public int key { get; set; }
		public decimal key_confidence { get; set; }
		public int mode { get; set; }
		public decimal mode_confidence { get; set; }
		public string codetext { get; set; }
		public decimal code_version { get; set; }
		public string echoprinttext { get; set; }
		public decimal echoprint_version { get; set; }
		public string synchtext { get; set; }
		public decimal synch_version { get; set; }
		public string rhythmtext { get; set; }
		public decimal rhythm_version { get; set; }
		public List<TrackBar> bars { get; set; }
		public List<TrackBeat> beats { get; set; }
		public List<TrackSection> sections { get; set; }
		public List<TrackSegment> segments { get; set; }
		public List<TrackTatum> tatums { get; set; }
		public AudioAnalysis()
        {
			bars = new List<TrackBar>();
			beats = new List<TrackBeat>();
			sections = new List<TrackSection>();
			segments = new List<TrackSegment>();
			tatums = new List<TrackTatum>();
		}			
	}
}
