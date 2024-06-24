using Microsoft.Maui.Controls;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SolutionApp
{
    public partial class MainPage : ContentPage

        {
            private readonly HttpClient _httpClient = new HttpClient();

            public MainPage()
            {
                InitializeComponent();
            }

            private async void OnFetchDataClicked(object sender, EventArgs e)
            {
                string firstName = firstNameEntry.Text;
                string year = YearEntry.Text;

                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(year))
                {
                    await DisplayAlert("Erreur", "Veuillez entrer un prenom et une année de recherche", "OK");
                    return;
                }

                var result = await FetchDataFromApi(firstName, year);
                ResultLabel.Text = result != null ? $"Le nombre d'enfants nommés {firstName} nés en {year} est de {result}" : "Aucune donnée trouvée";
            }

            private async Task<int?> FetchDataFromApi(string firstName, string year)
            {
                try
                {
                    string apiUrl = $"https://data.nantesmetropole.fr/explore/dataset/244400404_prenoms-enfants-nes-nantes/api/explore/v2.1/catalog/datasets/244400404_prenoms-enfants-nes-nantes/records?select=count(*)&where=enfant_prenom%3D'{firstName}'%20and%20annee%3D{year}&group_by=commune_nom&limit=20";
                    var response = await _httpClient.GetStringAsync(apiUrl);

                    // Désérialiser le JSON
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse>(response);
                    return apiResponse;
                }
                catch (Exception ex)
                {
                    // Gérer les erreurs d'appel API
                    await DisplayAlert("Erreur", $"Impossible de recuperer les données: {ex.Message}", "OK");
                    return null;
                }
            }

            public class ApiResponse
            {
                public Result[] Results { get; set; }
            }

            public class Result
            {
                public string CommuneNom { get; set; }
                public int Count { get; set; }
            }
        
    }

}
