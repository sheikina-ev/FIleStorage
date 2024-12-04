using FIleStorage.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;


namespace FIleStorage.Views
{
    public partial class UserProfile : ContentPage
    {
        private readonly User _user;
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
            UsernameLabel.Text = _user.Username;
            UsernameEntry.Text = _user.Username;
            EmailEntry.Text = _user.Email;
        }


        // ����� ��� ��������� ������� ������ "��������"
        private void OnEditButtonClicked(object sender, EventArgs e)
        {
          
        }
    }
}
