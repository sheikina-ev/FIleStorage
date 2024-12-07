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

        // �����������, ������� ��������� �����
        public UserProfile(User user, string token)
        {
            InitializeComponent();
            _user = user;
            _token = token;
            _httpClient = new HttpClient();

            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            // ��������� ������ ������������
            UsernameLabel.Text = _user.Username;
            UsernameEntry.Text = _user.Username;
            EmailEntry.Text = _user.Email;
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
