using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spotify.SpotifyModels
{
    public class Track
    {
        public int DiscNumber { get; set; }
        public int DurationMs { get; set; }
        public bool Explicit { get; set; }
        public string ExternalUrl { get; set; } = default!;
        public string Href { get; set; } = default!;
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Popularity { get; set; }
        public string PreviewUrl { get; set; } = default!;
        public int TrackNumber { get; set; }
        public AudioFeature audioFeatures { get; set; }
        public AudioAnalysis audioAnalysis { get; set; }
    }
    public class AudioFeature
    {
        public float Acousticness { get; set; }
        public string AnalysisUrl { get; set; } = default!;
        public float Danceability { get; set; }
        public int DurationMs { get; set; }
        public float Energy { get; set; }
        public float Instrumentalness { get; set; }
        public int Key { get; set; }
        public float Liveness { get; set; }
        public float Loudness { get; set; }
        public int Mode { get; set; }
        public float Speechiness { get; set; }
        public float Tempo { get; set; }
        public int TimeSignature { get; set; }
        public string TrackHref { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Uri { get; set; } = default!;
        public float Valence { get; set; }
    }
    public class AudioAnalysis
    {
        public Meta Meta { get; set; } = default!;
        public Analysis Analysis { get; set; } = default!;
        public List<TimeInterval> Bars { get; set; } = default!;
        public List<TimeInterval> Beats { get; set; } = default!;
        public List<Section> Sections { get; set; } = default!;
        public List<Segment> Segments { get; set; } = default!;
        public List<TimeInterval> Tatums { get; set; } = default!;
    }
    public class Meta
    {
        public string analyzer_version { get; set; }
        public string platform { get; set; }
        public string detailed_status { get; set; }
        public int status_code { get; set; }
        public int timestamp { get; set; }
        public double analysis_time { get; set; }
        public string input_process { get; set; }
    }
    public class Analysis
    {
        public int num_samples { get; set; }
        public double duration { get; set; }
        public string sample_md5 { get; set; }
        public int offset_seconds { get; set; }
        public int window_seconds { get; set; }
        public int analysis_sample_rate { get; set; }
        public int analysis_channels { get; set; }
        public double end_of_fade_in { get; set; }
        public double start_of_fade_out { get; set; }
        public double loudness { get; set; }
        public double tempo { get; set; }
        public double tempo_confidence { get; set; }
        public int time_signature { get; set; }
        public double time_signature_confidence { get; set; }
        public int key { get; set; }
        public double key_confidence { get; set; }
        public int mode { get; set; }
        public double mode_confidence { get; set; }
        public Analysis()
        {

        }
    }
    public class TimeInterval
    {
        public float Start { get; set; }
        public float Duration { get; set; }
        public float Confidence { get; set; }
    }
    public class Section
    {
        public float Start { get; set; }
        public float Duration { get; set; }
        public float Confidence { get; set; }
        public float Loudness { get; set; }
        public float Tempo { get; set; }
        public float TempoConfidence { get; set; }
        public int Key { get; set; }
        public float KeyConfidence { get; set; }
        public int Mode { get; set; }
        public float ModeConfidence { get; set; }
        public int TimeSignature { get; set; }
        public float TimeSignatureConfidence { get; set; }
    }
    public class Segment
    {
        public float Start { get; set; }
        public float Duration { get; set; }
        public float Confidence { get; set; }
        public float LoudnessStart { get; set; }
        public float LoudnessMax { get; set; }
        public float LoudnessMaxTime { get; set; }
        public float LoudnessEnd { get; set; }
        public List<float> Pitches { get; set; } = default!;
        public List<float> Timbre { get; set; } = default!;
    }
}
