using System;

namespace Spotify
{
    public class RelatedArtist
		{
			public int related_artist_id { get; set; }
			public DateTime? created_at { get; set; }
			public DateTime? modified_at { get; set; }
			public int artist_id { get; set; }
			public int related_id { get; set; }
			public string Id { get; set; }
		}

	
}
