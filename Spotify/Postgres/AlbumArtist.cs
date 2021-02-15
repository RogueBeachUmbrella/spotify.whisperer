using System;

namespace Spotify
{
    public class AlbumArtist
		{
			public int album_artist_id { get; set; }
			public DateTime? created_at { get; set; }
			public DateTime? modified_at { get; set; }
			public int artist_id { get; set; }
			public int album_id { get; set; }
		}

	
}
