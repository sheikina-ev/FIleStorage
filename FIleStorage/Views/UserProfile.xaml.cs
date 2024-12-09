using FIleStorage.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using FIleStorage.Views;

// ��������� ��������� ��� ������ ������ File
using MyFile = FIleStorage.Models.File;

namespace FIleStorage.Views
{
    public partial class UserProfile : ContentPage
    {
        private User _user;
        private readonly string _token;
        private readonly HttpClient _httpClient;
        private readonly UserService _userService;

        public UserProfile(User user, string token)
        {
            InitializeComponent();
            _user = user;
            _token = token;
            _httpClient = new HttpClient();
            _userService = new UserService(_httpClient, _token);

            LoadUserProfile();
            LoadUserFiles(); // ��������� ����� ������������ ����� ��� �������� �������
        }

        private void LoadUserProfile()
        {
            // ��������� ������ ������������
            UsernameLabel.Text = _user.Username;
            UsernameEntry.Text = _user.Username;
            EmailEntry.Text = _user.Email;
        }

        private async void LoadUserFiles()
        {
            try
            {
                var files = await _userService.GetUserFilesAsync();
                FilesListView.ItemsSource = files;

                // ��������� ���������� ����� �� ������
                FilesListView.ItemTapped += OnFileTapped;
            }
            catch (Exception ex)
            {
                // ��������� ������ (��������, �������� ��������� ������������)
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnFileTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is MyFile selectedFile)
            {
                await DisplayAlert("���������� � �����",
                    $"���: {selectedFile.Name}\n" +
                    $"����������: {selectedFile.Extension}\n" +
                    $"������: {selectedFile.Size}\n" +
                    $"����: {selectedFile.Path}\n" +
                    $"���� ��������: {selectedFile.CreatedAt?.ToString("g") ?? "�� �������"}",
                    "OK");
            }

            // ���������� ��������� ��������
            ((ListView)sender).SelectedItem = null;
        }


        // ����� ��� ��������� ������� ������ "��������"
        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            // ������� �� �������� ���������� ������� � ��������� ������ ������������
            var updateProfilePage = new UserUpdateProfilePage(_user, _token);

            // ������� ��������� �������, � ��������� ������ ����� �����������
            updateProfilePage.UserProfileUpdated += OnUserProfileUpdated;

            await Navigation.PushAsync(updateProfilePage);
        }

        private void OnUserProfileUpdated(User updatedUser)
        {
            // ��������� ��������� ������
            _user = updatedUser;

            // ������������� ����������� ������ �� ���� ��������
            LoadUserProfile();
        }
    }
}
