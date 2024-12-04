using FIleStorage.Models;
using System.Text;
using System.Text.Json;

namespace FIleStorage.Views;

public partial class UserUpdateProfilePage : ContentPage
{
    private readonly User _user;
    private readonly string _token;
    private readonly HttpClient _httpClient;

    public UserUpdateProfilePage(User user, string token)
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
        SurnameEntry.Text = _user.Surname;
        UsernameEntry.Text = _user.Username;
        EmailEntry.Text = _user.Email;
        PhoneEntry.Text = string.IsNullOrEmpty(_user.Phone) ? "" : _user.Phone;
        UsernameLabel.Text = _user.Username;
    }

    private async void OnUpdateButtonClicked(object sender, EventArgs e)
    {
        try
        {
            _user.Name = NameEntry.Text;
            _user.Surname = SurnameEntry.Text;
            _user.Email = EmailEntry.Text;
            _user.Phone = PhoneEntry.Text;

            var success = await UpdateUserProfileAsync();

            if (success)
            {
                await DisplayAlert("Успех", "Профиль успешно обновлен", "OK");
            }
            else
            {
                ErrorLabel.Text = "Не удалось обновить профиль. Попробуйте позже.";
                ErrorLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "OK");
        }
    }

    private async Task<bool> UpdateUserProfileAsync()
    {
        try
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(_user), Encoding.UTF8, "application/json");
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
