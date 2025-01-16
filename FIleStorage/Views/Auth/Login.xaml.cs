using FIleStorage.Models;
using FIleStorage.Utils;
using System.Text;
using System.Text.Json;
using FIleStorage.Views;

namespace FIleStorage.Views.Auth;

public partial class Login : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();

    public Login()
    {
        InitializeComponent();
    }

    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        // ������� �� �������� �����������
        await Shell.Current.GoToAsync("//RegisterPage");
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("������", "������� ��� ������������ � ������", "��");
            return;
        }

        var loginResponse = await AuthenticateUserAsync(username, password);

        if (loginResponse != null)
        {
            // ��������� ������ ������������ � �����
            UserData.User = loginResponse.User;
            UserData.Token = loginResponse.Token;

            // �������� ���������
            Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;

            // �������������� �� �������� �������
            await Navigation.PushAsync(new UserProfile());
        }
    }

    private async Task<AuthResponse> AuthenticateUserAsync(string username, string password)
    {
        var loginData = new { username, password };
        var jsonContent = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsync("http://course-project-4/api/login", jsonContent);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AuthResponse>(content);

                if (result?.Token != null)
                {
                    return result;
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await DisplayAlert("������ �����", "������������ ����� ��� ������", "OK");
            }
            else
            {
                await DisplayAlert("������", "��������� ������: " + response.ReasonPhrase, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("������", $"��������� ������: {ex.Message}", "OK");
        }
        return null;
    }
}
