using FIleStorage.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using FIleStorage.Views;

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
            }
            catch (Exception ex)
            {
                // ��������� ������ (��������, �������� ��������� ������������)
                await DisplayAlert("Error", ex.Message, "OK");
            }
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
