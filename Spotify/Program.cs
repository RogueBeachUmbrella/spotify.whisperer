using System;
using System.Collections.Generic;
using SpotifyAPI.Web;
using Newtonsoft.Json;


namespace Spotify
{
    class Program
    {
        static void Main(string[] args)
        {
            var spotify = new SpotifyClient("BQBg-8xIh4je-aCa6FpZOymuvvmxyI4QAoF1i876aNjpIX_nJjFb-T_hLnqDAm37LuyhZN9IVI9opS_2Gd6j5qJIVxbU_a2mV1iIqv6FGZjphJRz9odd4LJM5tCcPVmGye170mNuxR2BA-pe2A");

            //GetTracks(spotify);
            //GetTrackArtists(spotify);
            //GetArtistAlbums(spotify);
            GetArtistRelatedArtists(spotify);

        }


        static void GetTracks(SpotifyClient spotify)
        {
            var folder = @"C:\Users\jmgre\OneDrive\Documents\USF\4 - Advanced Database Management\Group Project\Spotify Data\api data\Tracks";
            var ids = new List<string>() {
                //"3IJYUaN3tx04S4TYyZHKoP", "19CSr8rwW05VJL2F91KFNK", "2Fxmhks0bxGSBdJ92vM42m", "78QR3Wp35dqAhFEc2qAGjE", "096YtKWNRXdmYKFLelSBto", "6J2LdBN97cDWn0MLxYh9HB", "7AQim7LbvFVZJE3O8TYgf2", "3jT2LKH0RSbQ8jIUNFzXm5", "2xLMifQCjDGFmkHkpNLD9h", "5IUtvfNvOyVYZUa6AJFrnP", "2IVsRhKrx8hlQBOWy4qebo", "3nS9a01VvXHQriLqJYwRqG", "6KfoDhO4XUWSbnyKjNp9c4", "2Z71PZlOeF9YVze4hy1A1a", "39Yp9wwQiSRIDOvrVg7mbk", "33gwZOGJWEZ7dRWPqPxBEZ", "0e7ipj03S05BNilyu5bRzt", "73YUReisjb3A9ActdLLjJQ", "07KXEDMj78x68D884wgVEm", "6MDdceLYec4AxohmorE4vH", "5wujBwqG7INdStqGd4tRMX", "0GO8y8jQk1PkHzS31d699N", "6eVxH9Kyanzrw636zJRPcw", "0vVMlbdYx2080Oa9FndPZr", "3ZCTVFBt2Brf31RLEnCkWJ", "2L4YgbxxwmwafMqLIjSx8q", "2wrJq5XKLnmhRXHIAf9xBa", "466cKvZn1j45IpxDdYZqdA", "003vvx7Niy0yvhvHt4a68B", "7yiSvALPjMrBLDDrbcDRNy", "75ZvA4QfFiZvzhj2xkaWAh", "5uCax9HTNlzGybIStD3vDh", "4wVOKKEHUJxHCFFNUWDn0B", "23T0OX7QOiIUFShSzbJ5Uo", "6ft4hAq6yde8jPZY2i5zLr", "4R2kfaDFhslZEMJqAFNpdd", "59qrUpoplZxbIZxk6X0Bm3", "6cx06DFPPHchuUAcTxznu9", "4vLBnQtece15fFhqWxZvJP", "1Z0cZI0UzNbP9L8MzzGxqf", "7dH9TPjTWQ60wOJkmYay8a", "6ocbgoVGwYJhOv1GgI9NsF", "7Jd0VVrzeTTWipxSzJ81aC", "2HbKqm4o0w5wEeEFXm2sD4", "7xQAfvXzm3AkraOtGPWIZg", "6XHVuErjQ4XNm6nDPVCxVX", "3oDkdAySo1VQQG0ptV7uwa", "3fqwjXwUGN6vbzIwvyFMhx", "0e4A9Fak1nJ7RtBD8YpoEo", "2QqJTIlGKRLJC3onkavYEz"
                //"1SF8piqsZWn86DajVKih8F", "0k7wmahjkn389wAZdz19Cv", "7vrJn5hDSXRmdXoR30KgF1", "7fEoXCZTZFosUFvFQg1BmW", "5dOxHHMOFAbG3VH23t0xNm", "1rgnBhdG2JDFTbYkYRZAku", "55Am8neGJkdj2ADaM3aw5H", "696DnlkuDOXcMAnKlTgXXK", "5CFJRZRq6sdKKtRwNPWbYv", "6Gg1gjgKi2AK4e0qzsR7sd", "6Ozh9Ok6h4Oi1wUSLtBseN", "79s5XnCN4TJKTVMSmOx8Ep", "3QzAOrNlsabgbMwlZt7TAY", "7ju97lgwC2rKQ6wwsf9no9", "45S5WTQEGOB1VHr1Q4FuPl", "7Bar1kLTmsRmH6FCKKMEyU", "7uoFMmxln0GPXQ0AcCBXRq", "5MwynWK9s4hlyKHqhkNn4A", "30bqVoKjX479ab90a8Pafp", "0ada5XsQGLPUVbmTCkAP49", "527k23H0A4Q0UJN3vGs0Da", "3eekarcy7kvN4yt5ZFzltW", "0GzuHFG4Ql6DoyxFRnIk3F", "3K07bGe8iljQ3mOKArHLDo", "48q0vSHcJdhK3IiXH8C5WJ", "4umIPjkehX1r7uhmGvXiSV", "0PurA4JVJ8YQgSVopY8fn6", "0u2P5u6lvoDfwTYjAADbn4", "5f1joOtoMeyppIcJGZQvqJ", "3Dv1eDb0MEgF93GpLXlucZ", "5RubKOuDoPn5Kj5TLVxSxY", "1xQ6trAsedVPCdbtDAmk0c", "6gdLyYNuhWBzqwlOFSsXC7", "3JKgcAa7r07ocVWcV8bS0H", "1eMNW1HQjF1dbb4GtnmpaX", "0uxSUdBrJy9Un0EYoBowng", "6wQlQrTY5mVS8EGaFZVwVF", "6gxKUmycQX7uyMwJcweFjp", "20McUFi8KCIVdNDLrkTDuZ", "1ZMiCix7XSAbfAJlEZWMCp", "3ee8Jmje8o58CHK66QrVC2", "5NvOZCjZaGGGL597exlQWv", "3GVkPk8mqxz0itaAriG1L7", "1H7KnK26kc1YyellpbINEn", "58ge6dfP91o9oXMzq3XkIS", "2rRJrJEo19S2J82BDsQ3F7", "2rxQMGVafnNaRaXlRMWPde", "2JvzF1RMd7lE3KmFlsyZD8", "7GX5flRQZVHRAGd6B4TmDO", "6K4t31amVTZDgR3sKmwUJJ"
                //"0RiRZpuVRbi7oqRdSMwhQY", "4saklk6nie3yiGePpBwUoc", "0ofHAoxe9vBkTCp2UQIavz", "4Iedi94TIaB2GGb1nMB68v", "7qEHsqek33rTcFNT9PFqLf", "53BHUFdQphHiZUUG3nx9zn", "5SWnsxjhdcEDc7LJjq9UHk", "2Oycxb8QbPkpHTo8ZrmG0B", "1K5KBOgreBi5fkEHvg5ap3", "0zLCBJZSiELJf02ucPP9wb", "5E30LdtzQTGqRvNd7l6kG5", "1VLtjHwRWOVJiE5Py7JxoQ", "2XIc1pqjXV3Cr2BQUGNBck", "2kS6td1yvmpNgZTt1q5pQq", "6ZuahEctZD6w75peme58hm", "43BlKpJcSrC9AsJ3F1DKg9", "7B3z0ySL9Rr0XvZEAjWZzM", "5u1n1kITHCxxp8twBcZxWy", "0eBXyY4SatzpE7opnzgXvz", "2gMXnyrvIjhVBUZwvLZDMP", "5VlTQnZO89Ioku8ssdbqJk", "3jjujdWJ72nww5eGnfs2E7", "4Q34FP1AT7GEl9oLgNtiWj", "0tbjiOUl4k492KPdWZS9sy", "4Z5FyQlevoHoa5FsIVKZju", "6NriykdkRrjQMZo1sfVYUo", "7o4gBbTM6UBLkOYPw9xMCz", "7m9OqQk4RVRkw9JJdeAw96", "7eJMfftS33KTjuF7lTsMCx", "5BK0uqwY9DNfZ630STAEaq", "4GBkffrtA51p17JH35irGA", "2rPHUAeUPbNgTmK18FPTiZ", "1IIKrJVP1C9N7iPtG6eOsK", "2mt1IqcFyY1zmYZT8Q3xw9", "3PfIrDoz19wz7qK7tYeu62", "7FIWs0pqAYbP91WWM0vlTQ", "7s95nPLMfiLTPoQ7pqUFmO", "5rZlwNFl01HqLWBQGryKSm", "0AUvWawuP0ibk4SQ3sIZjk", "0rKtyWc8bvkriBthvHKY8d", "6EDO9iiTtwNv6waLwa1UUq", "7bCfHiRcfUjG0YVVNUL7Ve", "1Cv1YLb4q0RzL6pybtaMLo", "6gBFPUFcJLzWGx4lenP6h2", "2GiJYvgVaD2HtM8GqD9EgQ", "62vpWI1CHwFy7tMIcSStl8", "57RA3JGafJm5zRtKJiKPIm", "6foY66mWZN0pSRjZ408c00", "1jaTQ3nqY3oAAYyCTbIvnM", "2kJwzbxV2ppxnQoYw4GLBZ"
                //"3YJJjQPAbDT7mGpX3WtQ9A", "31qCy5ZaophVA81wtlwLc4", "6Im9k8u9iIzKMrmV7BWtlF", "3tjFYV6RSFtuktYl3ZtYcq", "7hxHWCCAIIxFLCzvDgnQHX", "4MzXwWMhyBbmu6hOcLVD49", "35mvY5S1H3J2QZyna3TFe0", "5vGLcdRuSbUhD8ScwsGSdA", "27OeeYzk6klgBh83TSvGMA", "0VjIjW4GlUZAMYd2vXMi3b", "5nujrmhLynf4yMoMtj8AQF", "0PvFJmanyNQMseIFrU708S", "4Oun2ylbjFKMPTiaSbbCih", "3Uo7WG0vmLQ07WB4BDwy7D", "1tkg4EHVoqnhR6iFEXb60y", "54bFM56PmE4YLRnqpW6Tha", "0A1hoCfMLkiAgvhWkkucJa", "2SAqBLGA283SUiwJ3xOUVI", "6zFMeegAMYQo0mt8rXtrli", "7ytR5pFWmSjzHJIeQkgog4", "2U5WueTLIK5WJLD7mvDODv", "2QjOHCTQ1Jl3zawyYOpxh6", "22LAwLoDA5b4AaGSkg6bKW", "57VeLYXrvNxe8Vs18K2M9W", "2Y0wPrPQBrGhoLn14xRYCG", "3w1WjD2zJqjBjDz5fwqQPJ", "4VXIryQMWpIdGgYR4TrjT1", "5YSHygGN9D0mID1NsVd5my", "1HbA4N1MiOsPthALesGFR1", "0sY6ZUTh4yoctD8VIXz339", "0ZLuW8uOXdFNWcI40C0OC2", "2r6OAV3WsYtXuXjvJ1lIDi", "6UelLqGlWMcVH1E5c4H7lY", "0nbXyq5TXYPCO7pr3N8S4I", "21jGcNKet2qwijlDFuPiPb", "45bE4HXI0AwGZXfZtMp8JR", "7kDUspsoYfLkWnZR7qwHZl", "285pBltuF7vW8TeWk8hdRR", "3UoULw70kMsiVXxW0L3A33", "6IBcOGPsniK3Pso1wHIhew", "4xqrdfXkTW4T0RauPLv3WA", "6AGOKlMZWLCaEJGnaROtF9", "4aarlAfLKVCTxUDNgbwhjH", "4y4spB9m0Q6026KfkAvy9Q", "30KctD1WsHKTIYczXjip5a", "3qgPpwtuRu5oP8EtFSj8HE", "6Hj9jySrnFppAI0sEMCZpJ", "02MWAaffLxlfxAUY7c5dvx", "0SErdEdRcVX1uJCf1eTGYH", "0E4Y1XIbs8GrAT1YqVy6dq"
            };
            var request = new TracksRequest(ids);
            var tracks = spotify.Tracks.GetSeveral(request).Result.Tracks;
            tracks.ForEach(t => {
                System.IO.File.WriteAllText(@$"{folder}\track-{t.Id}.json", JsonConvert.SerializeObject(t));
            });
        }

        static void GetTrackArtists(SpotifyClient spotify)
        {
            string[] files = System.IO.Directory.GetFiles(@"C:\Users\jmgre\OneDrive\Documents\USF\4 - Advanced Database Management\Group Project\Spotify Data\api data\Tracks", "*.json");
            var ids = new List<string>();
            foreach(string f in files)
            {
                var track = JsonConvert.DeserializeObject<FullTrack>(System.IO.File.ReadAllText(@$"{f}"));
                track.Artists.ForEach(a => ids.Add(a.Id));
            }

            for(int i = 0; i <= ids.Count; i += 50)
            {
                var artists = spotify.Artists.GetSeveral(new ArtistsRequest(ids.GetRange(i, ((ids.Count - i) < 50 ? (ids.Count - i) : 50)))).Result.Artists;
                artists.ForEach(a => {
                    System.IO.File.WriteAllText(@$"C:\Users\jmgre\OneDrive\Documents\USF\4 - Advanced Database Management\Group Project\Spotify Data\api data\Artists\artist-{a.Id}.json", JsonConvert.SerializeObject(a));
                });
            }
        }

        static void GetArtistAlbums(SpotifyClient spotify)
        {
            string[] files = System.IO.Directory.GetFiles(@"C:\Users\jmgre\OneDrive\Documents\USF\4 - Advanced Database Management\Group Project\Spotify Data\api data\Artists", "*.json");
            var ids = new List<string>();
            foreach (string f in files)
            {
                var artist = JsonConvert.DeserializeObject<FullArtist>(System.IO.File.ReadAllText(@$"{f}"));
                spotify.Artists.GetAlbums(artist.Id).Result.Items.ForEach(a => {
                    System.IO.File.WriteAllText(@$"C:\Users\jmgre\OneDrive\Documents\USF\4 - Advanced Database Management\Group Project\Spotify Data\api data\Albums\album-{a.Id}.json", JsonConvert.SerializeObject(a));
                });
            }
        }

        static void GetArtistRelatedArtists(SpotifyClient spotify)
        {
            string[] files = System.IO.Directory.GetFiles(@"C:\Users\jmgre\OneDrive\Documents\USF\4 - Advanced Database Management\Group Project\Spotify Data\api data\Artists", "*.json");
            var ids = new List<string>();
            foreach (string f in files)
            {
                var artist = JsonConvert.DeserializeObject<FullArtist>(System.IO.File.ReadAllText(@$"{f}"));     
                System.IO.File.WriteAllText(@$"C:\Users\jmgre\OneDrive\Documents\USF\4 - Advanced Database Management\Group Project\Spotify Data\api data\Related Artists\related-{artist.Id}.json", JsonConvert.SerializeObject(spotify.Artists.GetRelatedArtists(artist.Id).Result.Artists));
            }
        }
    }
}
