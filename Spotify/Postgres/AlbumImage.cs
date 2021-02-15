using System;

namespace Spotify
{
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

	
}
