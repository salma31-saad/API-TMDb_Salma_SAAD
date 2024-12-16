using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using API_TMDb_Salma_SAAD.Model;
using Newtonsoft.Json;

namespace MovieSearchApp
{
    public partial class MainWindow : Window
    {
        // Clé d'API
        private const string ApiKey = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI3NmQ3NjhjYTZmM2FjNWIwNTVjZDE2ZmE1ZWZjZGZjZiIsIm5iZiI6MTczMjUyMjk1NS45MTM5OTk4LCJzdWIiOiI2NzQ0MzNjYjlhMmYzODJjYmQzNWY1YTAiLCJzY29wZXMiOlsiYXBpX3JlYWQiXSwidmVyc2lvbiI6MX0.Q3pOvfrr2oz2lM2CaWXR32VkMneYGCQilKObBsHxslg";
        List<Result> MoviesList;

        public MainWindow()
        {
            InitializeComponent();
            MoviesList = new List<Result>();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleInput.Text.Trim();
            string genre = GenreInput.Text.Trim();
            string actors = ActorsInput.Text.Trim();

            // Call SearchMovies with the inputs from the user
            await SearchMovies(title, genre, actors);
        }

        private async Task SearchMovies(string title, string genre, string actors)
        {
            // Start building the URL with the title
            StringBuilder url = new StringBuilder("https://api.themoviedb.org/3/search/movie?");

            // Add title to the search URL
            if (!string.IsNullOrWhiteSpace(title))
            {
                url.Append($"query={Uri.EscapeDataString(title)}");
            }

            // Add genre to the search URL if provided
            if (!string.IsNullOrWhiteSpace(genre))
            {
                // Recherche par genre (si genre connu ou ID du genre)
                url.Append($"&with_genres={Uri.EscapeDataString(genre)}");
            }

            // Ajouter les acteurs à la recherche, si fournis
            if (!string.IsNullOrWhiteSpace(actors))
            {
                // Recherche par acteur (doit utiliser l'ID d'un acteur)
                url.Append($"&with_cast={Uri.EscapeDataString(actors)}");
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
                    client.DefaultRequestHeaders.Add("accept", "application/json");

                    // Envoyer la requête à l'API
                    HttpResponseMessage response = await client.GetAsync(url.ToString());
                    response.EnsureSuccessStatusCode();

                    // Lire la réponse
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Root>(responseBody);

                    // Ajouter les films récupérés dans la liste
                    foreach (var movie in data.results)
                    {
                        // Ajouter l'URL complète de l'image du film
                        movie.poster_path = "https://image.tmdb.org/t/p/w500" + movie.poster_path;
                        MoviesList.Add(movie);
                    }

                    // Lier la liste des films au ListView
                    LvMovies.ItemsSource = MoviesList;
                    DataContext = this;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la recherche : {ex.Message}");
            }
        }
    }
}
