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
                    await DisplayAlert("������", "������� ��� ������������.", "OK");
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
                        await DisplayAlert("���������� ������", "������������ �� �������.", "OK");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("������", "������������ �� ���� �������!", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("������", $"��������� ������: {ex.Message}", "OK");
            }
        }

        private async void OnUserSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is User selectedUser)
            {
                var userInfo =
                    $"�������: {selectedUser.Surname}\n" +
                    $"���: {selectedUser.Name}\n" +
                    $"��� ������������: {selectedUser.Username}\n" +
                    $"Email: {selectedUser.Email}\n" +
                    $"�������: {selectedUser.Phone}";

                await DisplayAlert("���������� � ������������", userInfo, "OK");

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
                    await DisplayAlert("������", "������� �������� �����.", "OK");
                    return;
                }

                var requestBody = new { name = fileName };
                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                var response = await _httpClient.PostAsync($"http://course-project-4/api/search/file", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<Dictionary<string, List<File>>>(responseContent);

                    // ������� ����� ����������� ������
                    FilesFlexLayout.Children.Clear();

                    if (result != null && result.TryGetValue("files", out var files) && files.Count > 0)
                    {
                        foreach (var file in files)
                        {
                            Console.WriteLine($"���� ������: {file.Name}.{file.Extension}");

                            // ������� ��������� ��� �������� � ������
                            var fileContainer = new StackLayout
                            {
                                Orientation = StackOrientation.Vertical,    // ���������� ��������� �����������
                                HorizontalOptions = LayoutOptions.Center,   // ���������� ���������
                                VerticalOptions = LayoutOptions.Center,     // ���������� ���������
                                Spacing = 5
                            };

                            // ���������� ������ �����
                            var fileIcon = new Image
                            {
                                Source = "file_icon.png",
                                WidthRequest = 50,
                                HeightRequest = 50,
                                BackgroundColor = Colors.Transparent,
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center
                            };

                            // ���������� ����� � ��������� �����
                            var fileLabel = new Label
                            {
                                Text = $"{file.Name}.{file.Extension}",
                                TextColor = Colors.White,
                                FontSize = 18,
                                HorizontalTextAlignment = TextAlignment.Center,
                                Margin = new Thickness(0, 5)
                            };

                            // ���������� ����������� �������
                            var tapGesture = new TapGestureRecognizer();
                            tapGesture.Command = new Command(async (param) => await OnFileSelected(param as File));
                            tapGesture.CommandParameter = file;
                            fileLabel.GestureRecognizers.Add(tapGesture);

                            // ���������� ��������� � ���������
                            fileContainer.Children.Add(fileIcon);
                            fileContainer.Children.Add(fileLabel);

                            // ���������� ���������� � FlexLayout
                            FilesFlexLayout.Children.Add(fileContainer);
                        }
                    }
                    else
                    {
                        FilesFlexLayout.Children.Add(new Label
                        {
                            Text = "����� �� �������.",
                            TextColor = Colors.White,
                            FontSize = 18,
                            HorizontalTextAlignment = TextAlignment.Center
                        });
                    }
                }
                else
                {
                    await DisplayAlert("������", "�� ������� �������� �����.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("������", $"��������� ������: {ex.Message}", "OK");
            }
        }

        private async Task OnFileSelected(File file)
        {
            if (file == null)
            {
                await DisplayAlert("������", "�� ������� ��������� ���������� � �����.", "OK");
                return;
            }

            await DisplayAlert("���������� � �����",
                $"���: {file.Name}\n" +
                $"����������: {file.Extension}\n" +
                $"������: {file.Size}\n" +
                $"����: {file.Path}\n" +
                $"���� ��������: {file.CreatedAt?.ToString("g") ?? "�� �������"}",
                "OK");
        }
    }
}
