using FIleStorage.Models;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using File = FIleStorage.Models.File;
using FIleStorage.Utils;

namespace FIleStorage.Views
{
    public partial class SearchPage : ContentPage
    {
        private readonly User _currentUser; 
        private readonly string _token;   
        private readonly HttpClient _httpClient;

        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

        public ObservableCollection<File> Files { get; set; } = new ObservableCollection<File>();

        public SearchPage()
        {
            InitializeComponent();

            _currentUser = UserData.User;
            _token = UserData.Token;
            _httpClient = new HttpClient();

            BindingContext = this; 
        }

        private async void OnSearchButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var username = SearchEntry.Text;
                if (string.IsNullOrWhiteSpace(username))
                {
                    await DisplayAlert("Ошибка", "Введите имя пользователя.", "OK");
                    return;
                }

                var requestBody = new { username };
                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                var response = await _httpClient.PostAsync("http://course-project-4/api/search/user", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<Dictionary<string, List<User>>>(responseContent);

                    if (result != null && result.TryGetValue("users", out var users) && users.Count > 0)
                    {
                        Users.Clear();
                        foreach (var user in users)
                        {
                            Users.Add(user);
                        }
                    }
                    else
                    {
                        await DisplayAlert("Результаты поиска", "Пользователи не найдены.", "OK");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Ошибка", "Пользователи не были найдены!", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "OK");
            }
        }
        private async void OnUserSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is User selectedUser)
            {
                var userInfo =
                    $"Фамилия: {selectedUser.Surname}\n" +
                    $"Имя: {selectedUser.Name}\n" +
                    $"Имя пользователя: {selectedUser.Username}\n" +
                    $"Email: {selectedUser.Email}\n" +
                    $"Телефон: {selectedUser.Phone}";

                await DisplayAlert("Информация о пользователе", userInfo, "OK");

                ((ListView)sender).SelectedItem = null;
            }
        }


        private async void OnSearchFileButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var fileName = FileSearchEntry.Text?.Trim();
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    await DisplayAlert("Ошибка", "Введите название файла.", "OK");
                    return;
                }

                var requestBody = new { name = fileName };
                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Установка токена авторизации
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                var response = await _httpClient.PostAsync($"http://course-project-4/api/search/file", content);

                // Получение ответа
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Десериализация ответа
                    var result = JsonSerializer.Deserialize<Dictionary<string, List<File>>>(responseContent);

                    // Очищаем StackLayout перед добавлением новых файлов
                    FilesStackLayout.Children.Clear();

                    if (result != null && result.TryGetValue("files", out var files) && files.Count > 0)
                    {
                        foreach (var file in files)
                        {
                            Console.WriteLine("files", files);
                            // Создаем новый Label для каждого найденного файла
                            var fileLabel = new Label
                            {
                                Text = $"{file.Name}.{file.Extension}",

                                TextColor = Colors.White,
                                FontSize = 18,
                                Margin = new Thickness(0, 5)
                            };

                            // Добавляем Label в StackLayout
                            FilesStackLayout.Children.Add(fileLabel);
                        }
                    }
                    else
                    {
                        // Добавляем сообщение, что файлы не найдены
                        var noFilesLabel = new Label
                        {
                            Text = "Файлы не найдены.",
                            TextColor = Colors.White,
                            FontSize = 18,
                            Margin = new Thickness(0, 5)
                        };

                        FilesStackLayout.Children.Add(noFilesLabel);
                    }
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось получить файлы.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "OK");
            }
        }

    }
}