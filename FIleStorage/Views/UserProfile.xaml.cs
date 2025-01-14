using FIleStorage.Models;
using FIleStorage.Utils;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

// ��������� ��������� ��� ������ File
using MyFile = FIleStorage.Models.File;
using System.IO;

namespace FIleStorage.Views
{
    public partial class UserProfile : ContentPage
    {
        private User _user;
        private readonly string _token;
        private readonly HttpClient _httpClient;
        private readonly UserService _userService;

        public UserProfile()
        {
            
            InitializeComponent();


            _user = UserData.User;
            _token = UserData.Token;
            _httpClient = new HttpClient();
            _userService = new UserService(_httpClient, _token);

            LoadUserProfile();
            LoadUserFiles(); // ��������� ����� ������������ ��� �������� ��������
        }

        private void LoadUserProfile()
        {
            // ��������� ������ ������������
            UsernameLabel.Text = _user.Username;
            UsernameStaticLabel.Text = _user.Username;
            EmailStaticLabel.Text = _user.Email;
        }

        private async void LoadUserFiles()
        {
            try
            {
                // �������� ��� ����� ������������
                var files = await _userService.GetUserFilesAsync();

                // ����� ������ ������ ��� �����
                var limitedFiles = files.Take(3).ToList();

                // ������� ���������
                FilesContainer.Children.Clear();

                // ���� ������ ���, ���������� ���������
                if (!limitedFiles.Any())
                {
                    FilesContainer.Children.Add(new Label
                    {
                        Text = "��� ��������� ������. �� ������ ��������� ����� ����� ��������������� ������.",
                        TextColor = Colors.White,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontSize = 16,
                        Padding = new Thickness(20)
                    });
                    return;
                }

                // ��������� ����� � ���������
                foreach (var file in limitedFiles)
                {
                    var fileStack = new VerticalStackLayout
                    {
                        Spacing = 5,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    };

                    var fileButton = new ImageButton
                    {
                        Source = "file_icon.png",
                        WidthRequest = 80,
                        HeightRequest = 80,
                        BackgroundColor = Colors.Transparent,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    };

                    fileButton.Clicked += (s, e) => OnFileSelected(file);

                    var fileNameLabel = new Label
                    {
                        Text = file.Name,
                        FontSize = 14,
                        TextColor = Colors.White,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center
                    };

                    fileStack.Children.Add(fileButton);
                    fileStack.Children.Add(fileNameLabel);

                    FilesContainer.Children.Add(fileStack);
                }
            }
            catch (Exception ex)
            {
                // �������� ������ � ������� ��� ��������� ��������� �� ��������
                Console.WriteLine($"������ ��� �������� ������: {ex.Message}");

                // ���������� ��������� �� ������ �� ��������, ���� ���������
                FilesContainer.Children.Clear();
                FilesContainer.Children.Add(new Label
                {
                    Text = "��� ��������� ������",
                    TextColor = Colors.Red,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = 16,
                    Padding = new Thickness(20)
                });
            }
        }



        private async void OnFileSelected(MyFile file)
        {
            await DisplayAlert("���������� � �����",
                $"���: {file.Name}\n" +
                $"����������: {file.Extension}\n" +
                $"������: {file.Size}\n" +
                $"����: {file.Path}\n" +
                $"���� ��������: {file.CreatedAt?.ToString("g") ?? "�� �������"}",
                "OK");
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            var updateProfilePage = new UserUpdateProfilePage();

            updateProfilePage.UserProfileUpdated += OnUserProfileUpdated;

            await Navigation.PushAsync(updateProfilePage);
        }

        private void OnUserProfileUpdated(User updatedUser)
        {
            _user = updatedUser;
            LoadUserProfile();
        }

        private async void OnShowAllFilesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FilesPage());
        }
        private async void OnNavigateToFilesPage(object sender, EventArgs e)
        {
            // ������� �� �������� "�����"
            await Shell.Current.GoToAsync("//filesPage");
        }
       
      

    }
}
