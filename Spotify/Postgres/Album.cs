using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spotify
{
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
		public List<Track> tracks { get; set; }

		public Album()
        {
			tracks = new List<Track>();
        }
		public void GetTracks(SpotifyClient client)
        {
            var responseA = client.Albums.GetTracks(id).Result;

            var trackIds = client.Albums.GetTracks(id).Result.Items.Select(t => t.Id).ToList();
            for (int i = 0; i < trackIds.Count; i += (trackIds.Count - i) >= 50 ? 50 : (trackIds.Count - i))
            {
                var request = new TracksRequest(trackIds.GetRange(i, ((trackIds.Count - i) >= 50 ? 50 : (trackIds.Count - i))));
                var response = client.Tracks.GetSeveral(request).Result.Tracks;
                response.ForEach(t =>
                {
                    var track = new Track()
                    {
                        duration_ms = t.DurationMs,
                        isexplicit = t.Explicit,
                        name = t.Name,
                        popularity = t.Popularity,
                        preview_url = t.PreviewUrl,
                        track_number = t.TrackNumber,
                        type = t.Type.ToString(),
                        uri = t.Uri,
                        spotify_url = t.ExternalUrls.Values.FirstOrDefault(),
                        id = t.Id,
                        disc_number = t.DiscNumber
                    };
                    tracks.Add(track);
                });
            };
        }
        public List<string> GetTrackIds()
        {         
            return tracks.Select(t => t.id).ToList();
        }
        public bool HasTrack(string trackId)
        {
            return GetTrackIds().Contains(trackId);
        }
        public void UpdateTrackFeature(TrackAudioFeatures f)
        {
            var trackFeature = new TrackFeatures()
            {
                danceability = (decimal)f.Danceability,
                energy = (decimal)f.Energy,
                key = f.Key,
                loudness = (decimal)f.Loudness,
                mode = f.Mode,
                speechiness = (decimal)f.Speechiness,
                acousticness = (decimal)f.Acousticness,
                instrumentalness = (decimal)f.Instrumentalness,
                liveness = (decimal)f.Liveness,
                valence = (decimal)f.Valence,
                tempo = (decimal)f.Tempo,
                duration_ms = f.DurationMs,
                time_signature = f.TimeSignature
            };
            tracks.Where(track => track.id == f.Id).Single().features = trackFeature;
        }
        public void UpdateTrackAnalysis(string trackId, TrackAudioAnalysis trackAudioAnalysis)
        {
            var audioAnalysis = new AudioAnalysis()
            {
                num_samples = trackAudioAnalysis.Analysis.num_samples,
                duration = (decimal)trackAudioAnalysis.Analysis.duration,
                sample_md5 = trackAudioAnalysis.Analysis.sample_md5,
                offset_seconds = trackAudioAnalysis.Analysis.offset_seconds,
                window_seconds = trackAudioAnalysis.Analysis.window_seconds,
                analysis_sample_rate = trackAudioAnalysis.Analysis.analysis_sample_rate,
                analysis_channels = trackAudioAnalysis.Analysis.analysis_channels,
                end_of_fade_in = (decimal)trackAudioAnalysis.Analysis.end_of_fade_in,
                start_of_fade_out = (decimal)trackAudioAnalysis.Analysis.start_of_fade_out,
                loudness = (decimal)trackAudioAnalysis.Analysis.loudness,
                tempo = (decimal)trackAudioAnalysis.Analysis.tempo,
                tempo_confidence = (decimal)trackAudioAnalysis.Analysis.tempo_confidence,
                time_signature = trackAudioAnalysis.Analysis.time_signature,
                time_signature_confidence = (decimal)trackAudioAnalysis.Analysis.time_signature_confidence,
                key = trackAudioAnalysis.Analysis.key,
                key_confidence = (decimal)trackAudioAnalysis.Analysis.key_confidence,
                mode = trackAudioAnalysis.Analysis.mode,
                mode_confidence = (decimal)trackAudioAnalysis.Analysis.mode_confidence
            };
            trackAudioAnalysis.Bars.ForEach(b => {
                var bar = new TrackBar()
                {
                    start = (decimal)b.Start,
                    duration = (decimal)b.Duration,
                    confidence = (decimal)b.Confidence
                };
                audioAnalysis.bars.Add(bar);
            });
            trackAudioAnalysis.Beats.ForEach(b => {
                var beat = new TrackBeat()
                {
                    start = (decimal)b.Start,
                    duration = (decimal)b.Duration,
                    confidence = (decimal)b.Confidence
                };
                audioAnalysis.beats.Add(beat);
            });
            trackAudioAnalysis.Sections.ForEach(s => {
                var section = new TrackSection()
                {
                    start = (decimal)s.Start,
                    duration = (decimal)s.Duration,
                    confidence = (decimal)s.Confidence,
                    loudness = (decimal)s.Loudness,
                    tempo = (decimal)s.Tempo,
                    tempo_confidence = (decimal)s.TempoConfidence,
                    key = s.Key,
                    key_confidence = (decimal)s.KeyConfidence,
                    mode = s.Mode,
                    mode_confidence = (decimal)s.ModeConfidence,
                    time_signature = s.TimeSignature,
                    time_signature_confidence = (decimal)s.TimeSignatureConfidence
                };
                audioAnalysis.sections.Add(section);
            });
            trackAudioAnalysis.Segments.ForEach(s => {
                var segment = new TrackSegment()
                {
                    start = (decimal)s.Start,
                    duration = (decimal)s.Duration,
                    confidence = (decimal)s.Confidence,
                    loudness_start = (decimal)s.LoudnessStart,
                    loudness_max_time = (decimal)s.LoudnessMaxTime,
                    loudness_max = (decimal)s.LoudnessMax,
                    loudness_end = (decimal)s.LoudnessEnd
                };
                s.Pitches.ForEach(pitch => { segment.pitches.Add(new Pitch() { pitch = (decimal)pitch }); });
                s.Timbre.ForEach(timbre => { segment.timbres.Add(new Timbre() { timbre = (decimal)timbre }); });
                audioAnalysis.segments.Add(segment);
            });
            trackAudioAnalysis.Tatums.ForEach(t => {
                var tatum = new TrackTatum()
                {
                    start = (decimal)t.Start,
                    duration = (decimal)t.Duration,
                    confidence = (decimal)t.Confidence
                };
                audioAnalysis.tatums.Add(tatum);
            });
            tracks.Where(track => track.id == trackId).Single().audioAnalysis = audioAnalysis;
        }
    }  
}

