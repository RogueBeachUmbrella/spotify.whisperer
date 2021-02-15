using System;

namespace Spotify
{
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

	
}
