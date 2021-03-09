using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spotify.SpotifyModels
{
    public class Track
    {
        [JsonProperty("disc_number")] 
        public int DiscNumber { get; set; }
        
        [JsonProperty("duration_ms")] 
        public int DurationMs { get; set; }
        
        [JsonProperty("isexplicit")] 
        public bool Explicit { get; set; }
        
        [JsonProperty("spotify_url")] 
        public string ExternalUrl { get; set; } = default!;
        
        [JsonProperty("_id")] 
        public string Id { get; set; } = default!;
        
        [JsonProperty("name")] 
        public string Name { get; set; } = default!;
        
        [JsonProperty("populrity")] 
        public int? Popularity { get; set; }
        
        [JsonProperty("preview_url")] 
        public string PreviewUrl { get; set; } = default!;
        
        [JsonProperty("track_numbr")] 
        public int TrackNumber { get; set; }
        
        [JsonProperty("artist_ids")] 
        public List<string> artistIds { get; set; }
        
        [JsonProperty("album_id")] 
        public string albumId { get; set; }
        
        [JsonProperty("audio_features")] 
        public AudioFeature audioFeatures { get; set; }
        
        [JsonProperty("audio_analysis")] 
        public AudioAnalysis audioAnalysis { get; set; }
    }
    public class AudioFeature
    {
        
        [JsonProperty("acousticness")] 
        public float Acousticness { get; set; }
        
        [JsonProperty("analysis_url")] 
        public string AnalysisUrl { get; set; } = default!;
        
        [JsonProperty("danceability")] 
        public float Danceability { get; set; }
        
        [JsonProperty("duration_ms")] 
        public int DurationMs { get; set; }
        
        [JsonProperty("energy")] 
        public float Energy { get; set; }
        
        [JsonProperty("instrumentalness")] 
        public float Instrumentalness { get; set; }
        
        [JsonProperty("key")] 
        public int Key { get; set; }
        
        [JsonProperty("liveness")] 
        public float Liveness { get; set; }
        
        [JsonProperty("loudness")] 
        public float Loudness { get; set; }
        
        [JsonProperty("mode")] 
        public int Mode { get; set; }
        
        [JsonProperty("speechiness")] 
        public float Speechiness { get; set; }
        
        [JsonProperty("tempo")] 
        public float Tempo { get; set; }
        
        [JsonProperty("time_signature")] 
        public int TimeSignature { get; set; }
        
        //[JsonProperty("")] 
        //public string TrackHref { get; set; } = default!;
        
        //[JsonProperty("")] 
        //public string Type { get; set; } = default!;
        
        //[JsonProperty("")] 
        //public string Uri { get; set; } = default!;
        
        [JsonProperty("valence")] 
        public float Valence { get; set; }
    }
    public class AudioAnalysis
    {
        
        [JsonProperty("meta")] 
        public Meta Meta { get; set; } = default!;
        
        [JsonProperty("analysis")] 
        public Analysis Analysis { get; set; } = default!;
        
        [JsonProperty("bars")] 
        public List<TimeInterval> Bars { get; set; } = default!;
        
        [JsonProperty("beats")] 
        public List<TimeInterval> Beats { get; set; } = default!;
        
        [JsonProperty("sections")] 
        public List<Section> Sections { get; set; } = default!;
        
        [JsonProperty("segments")] 
        public List<Segment> Segments { get; set; } = default!;
        
        [JsonProperty("tatums")] 
        public List<TimeInterval> Tatums { get; set; } = default!;
    }
    public class Meta
    {
        
        [JsonProperty("analyzer_version")] 
        public string analyzer_version { get; set; }
        
        [JsonProperty("platform")] 
        public string platform { get; set; }
        
        [JsonProperty("detailed_status")] 
        public string detailed_status { get; set; }
        
        [JsonProperty("status_code")] 
        public int status_code { get; set; }
        
        [JsonProperty("timestamp")] 
        public int timestamp { get; set; }
        
        [JsonProperty("analysis_time")] 
        public double analysis_time { get; set; }
        
        [JsonProperty("input_process")] 
        public string input_process { get; set; }
    }
    public class Analysis
    {
        
        [JsonProperty("num_samples")] 
        public int num_samples { get; set; }
        
        [JsonProperty("duration")] 
        public double duration { get; set; }
        
        [JsonProperty("sample_md5")] 
        public string sample_md5 { get; set; }
        
        [JsonProperty("offset_seconds")] 
        public int offset_seconds { get; set; }
        
        [JsonProperty("window_seconds")] 
        public int window_seconds { get; set; }
        
        [JsonProperty("analysis_sample_rate")] 
        public int analysis_sample_rate { get; set; }
        
        [JsonProperty("analysis_channels")] 
        public int analysis_channels { get; set; }
        
        [JsonProperty("end_of_fade_in")] 
        public double end_of_fade_in { get; set; }
        
        [JsonProperty("start_of_fade_out")] 
        public double start_of_fade_out { get; set; }
        
        [JsonProperty("loudness")] 
        public double loudness { get; set; }
        
        [JsonProperty("tempo")] 
        public double tempo { get; set; }
        
        [JsonProperty("tempo_confidence")] 
        public double tempo_confidence { get; set; }
        
        [JsonProperty("time_signature")] 
        public int time_signature { get; set; }
        
        [JsonProperty("time_signature_confidence")] 
        public double time_signature_confidence { get; set; }
        
        [JsonProperty("key")] 
        public int key { get; set; }
        
        [JsonProperty("key_confidence")] 
        public double key_confidence { get; set; }
        
        [JsonProperty("mode")] 
        public int mode { get; set; }
        
        [JsonProperty("mode_confidence")] 
        public double mode_confidence { get; set; }
    }
    public class TimeInterval
    {
        
        [JsonProperty("start")] 
        public float Start { get; set; }
        
        [JsonProperty("duration")] 
        public float Duration { get; set; }
        
        [JsonProperty("confidence")] 
        public float Confidence { get; set; }
    }
    public class Section
    {
        
        [JsonProperty("start")] 
        public float Start { get; set; }
        
        [JsonProperty("duration")] 
        public float Duration { get; set; }
        
        [JsonProperty("confidence")] 
        public float Confidence { get; set; }
        
        [JsonProperty("loudness")] 
        public float Loudness { get; set; }
        
        [JsonProperty("tempo")] 
        public float Tempo { get; set; }
        
        [JsonProperty("tempo_confidence")] 
        public float TempoConfidence { get; set; }
        
        [JsonProperty("key")] 
        public int Key { get; set; }
        
        [JsonProperty("key_confidence")] 
        public float KeyConfidence { get; set; }
        
        [JsonProperty("mode")] 
        public int Mode { get; set; }
        
        [JsonProperty("mode_confidence")] 
        public float ModeConfidence { get; set; }
        
        [JsonProperty("time_signature")] 
        public int TimeSignature { get; set; }
        
        [JsonProperty("time_signature_confidence")] 
        public float TimeSignatureConfidence { get; set; }
    }
    public class Segment
    {
        
        [JsonProperty("start")] 
        public float Start { get; set; }
        
        [JsonProperty("duration")] 
        public float Duration { get; set; }
        
        [JsonProperty("confidence")] 
        public float Confidence { get; set; }
        
        [JsonProperty("loudness_start")] 
        public float LoudnessStart { get; set; }
        
        [JsonProperty("loudness_max")] 
        public float LoudnessMax { get; set; }
        
        [JsonProperty("loudness_max_time")] 
        public float LoudnessMaxTime { get; set; }
        
        [JsonProperty("loudness_end")] 
        public float LoudnessEnd { get; set; }
        
        [JsonProperty("pitches")] 
        public List<float> Pitches { get; set; } = default!;
        
        [JsonProperty("timbres")] 
        public List<float> Timbre { get; set; } = default!;
    }
}
