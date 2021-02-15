using System;

namespace Spotify
{
    public class TrackAlbum
		{
			public int track_album_id { get; set; }
			public DateTime? created_at { get; set; }
			public DateTime? modified_at { get; set; }
			public int track_id { get; set; }
			public int album_id { get; set; }
			public int disc_number { get; set; }
		}

	
}
