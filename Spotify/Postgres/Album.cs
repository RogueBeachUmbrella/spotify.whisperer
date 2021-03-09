using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Postgres
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

		//public Album()
  //      {
		//	tracks = new List<Track>();
  //      }
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
    }  
}

