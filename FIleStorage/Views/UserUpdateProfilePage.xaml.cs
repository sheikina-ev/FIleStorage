using FIleStorage.Models;
using System.Text;
using System.Text.Json;

namespace FIleStorage.Views;

public partial class UserProfilePage : ContentPage
{
    private readonly User _user;
    private readonly string _token;
    private readonly HttpClient _httpClient;

    public UserProfilePage(User user, string token)
    {
        InitializeComponent();

        _user = user; 
        _token = token; 
        _httpClient = new HttpClient();

        LoadUserProfile(); 
    }

    private void LoadUserProfile()
    {
        NameEntry.Text = _user.Name;
        UsernameEntry.Text = _user.Username;
        EmailEntry.Text = _user.Email;
    }

    private async void OnUpdateButtonClicked(object sender, EventArgs e)
    {
        try
        {
            _user.Name = NameEntry.Text; // ��������� ���
            _user.Email = EmailEntry.Text; // ��������� email

            // ���������� ������ �� ������
            var success = await UpdateUserProfileAsync();

            if (success)
            {
                await DisplayAlert("�����", "������� ������� ��������", "OK");
            }
            else
            {
                ErrorLabel.Text = "�� ������� �������� �������. ���������� �����.";
                ErrorLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("������", $"��������� ������: {ex.Message}", "OK");
        }
    }

    private async Task<bool> UpdateUserProfileAsync()
    {
        try
        {
            // ��������� JSON-������
            var jsonContent = new StringContent(JsonSerializer.Serialize(_user), Encoding.UTF8, "application/json");

            // ��������� ����� � ���������
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            
            var response = await _httpClient.PostAsync("http://course-project-4/api/profile", jsonContent);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false; 
        }
    }
}
