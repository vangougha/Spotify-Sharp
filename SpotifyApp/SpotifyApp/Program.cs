using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SpotifyApiExample
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            try
            {
                var denemeId = "1VPmR4DJC1PlOtd0IADAO0";
                var accessToken = "BQCy2B7BaNDra6T7L7cvCygfRbVLtlnfYyc_I2wJrbuxnEwlpENoRBb5B-EgXTTqs_eCYIWCwyVjQ803JxmOkHiWBSTWfTY8vwumPBjSNCWrYIxsH88";

                // Sanatçı bilgilerini al
                string artistInfo = await GetArtistInfoAsync(denemeId, accessToken);
                Console.WriteLine("Artist Info: " + artistInfo);

                // Yeni çıkan albümleri al
                string json = await GetSpotifyDataAsync(accessToken);

                // Albümleri ayıkla
                string[] albumInfo = ParseAlbums(json);
                foreach (string info in albumInfo)
                {
                    Console.WriteLine(info);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static async Task<string> GetArtistInfoAsync(string artistId, string accessToken)
        {
            string artistUrl = $"https://api.spotify.com/v1/artists/{artistId}";

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            HttpResponseMessage response = await client.GetAsync(artistUrl);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            return jsonResponse;
        }

        private static async Task<string> GetSpotifyDataAsync(string accessToken)
        {
            // Spotify API URL ve access token'ı buraya ekle
            string apiUrl = "https://api.spotify.com/v1/browse/new-releases";

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            return json;
        }

        private static string[] ParseAlbums(string json)
        {
            JObject data = JObject.Parse(json);
            JArray albumsArray = (JArray)data["albums"]["items"];
            string[] albums = new string[albumsArray.Count];

            for (int i = 0; i < albumsArray.Count; i++)
            {
                JObject album = (JObject)albumsArray[i];
                string name = album["name"].ToString();
                string releaseDate = album["release_date"].ToString();
                string artistName = album["artists"][0]["name"].ToString();
                string albumUrl = album["external_urls"]["spotify"].ToString();
                string albumImageUrl = album["images"][0]["url"].ToString();

                albums[i] = $"Album Name: {name}, Release Date: {releaseDate}, Artist: {artistName}, Album URL: {albumUrl}, Album Image URL: {albumImageUrl}";
            }

            return albums;
        }
    }
}
