using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify
{
	static class PgSQL
    {
		public class Artist
		{
			public int artist_id { get; set; }
			public DateTime? created_at { get; set; }
			public DateTime? modified_at { get; set; }
			public string id { get; set; }
			public string name { get; set; }
			public int popularity { get; set; }
			public string type { get; set; }
			public string uri { get; set; }
			public int followers { get; set; }
			public string spotify_url { get; set; }
		}
		public class ArtistImage
		{
			public int artist_image_id { get; set; }
			public DateTime? created_at { get; set; }
			public DateTime? modified_at { get; set; }
			public int artist_id { get; set; }
			public int height { get; set; }
			public string url { get; set; }
			public int width { get; set; }
		}

		public class RelatedArtist
		{
			public int related_artist_id { get; set; }
			public DateTime? created_at { get; set; }
			public DateTime? modified_at { get; set; }
			public int artist_id { get; set; }
			public int related_id { get; set; }
		}

		public class ArtistGenre
		{
			public int artist_genre_id { get; set; }
			public DateTime? created_at { get; set; }
			public DateTime? modified_at { get; set; }
			public int artist_id { get; set; }
			//genre genre { get; set; }
		}

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
		}

		public class AlbumArtist
		{
			public int album_artist_id { get; set; }
			public DateTime? created_at { get; set; }
			public DateTime? modified_at { get; set; }
			public int artist_id { get; set; }
			public int album_id { get; set; }
		}

		public class AlbumImage
		{
			public int album_image_id { get; set; }
			public DateTime? created_at { get; set; }
			public DateTime? modified_at { get; set; }
			public int album_id { get; set; }
			public int height { get; set; }
			public string url { get; set; }
			public int width { get; set; }
		}

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
		}

		public class TrackArtist
		{
			public int track_artist_id { get; set; }
			public DateTime? created_at { get; set; }
			public DateTime? modified_at { get; set; }
			public int track_id { get; set; }
			public int artist_id { get; set; }
		}

		public class TrackAlbum
		{
			public int track_album_id { get; set; }
			public DateTime? created_at { get; set; }
			public DateTime? modified_at { get; set; }
			public int track_id { get; set; }
			public int album_id { get; set; }
			public int disc_number { get; set; }
		}

		public class TrackFeature
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

		public class TrackAudioAnalysis
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
		}

		public class TrackBar
		{
			public int track_bar_id { get; set; }
			public DateTime? created_at { get; set; }
			public DateTime? modified_at { get; set; }
			public int track_id { get; set; }
			public decimal start { get; set; }
			public decimal duration { get; set; }
			public decimal confidence { get; set; }
		}

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

		public class TrackTatum
		{
			public int track_tatum_id { get; set; }
			public DateTime? created_at { get; set; }
			public DateTime? modified_at { get; set; }
			public int track_id { get; set; }
			public decimal start { get; set; }
			public decimal duration { get; set; }
			public decimal confidence { get; set; }
		}

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
		}

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
		}

		public class TrackSegmentPitch
		{
			public int track_segment_pitch_id { get; set; }
			public DateTime? created_at { get; set; }
			public DateTime? modified_at { get; set; }
			public int track_segment_id { get; set; }
			public decimal pitch { get; set; }
		}

		public class TrackSegmentTimbre
		{
			public int track_segment_timbre_id { get; set; }
			public DateTime? created_at { get; set; }
			public DateTime? modified_at { get; set; }
			public int track_segment_id { get; set; }
			public decimal timbre { get; set; }
		}

	}
}
