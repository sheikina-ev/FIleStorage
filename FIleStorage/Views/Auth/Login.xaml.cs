using FIleStorage.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        await Navigation.PushAsync(new Register());
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

            // ����� ��������� ����� �������������� ������������ �� �������� �������
            await Navigation.PushAsync(new UserProfile(loginResponse.User,loginResponse.Token));
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
                await DisplayAlert("������", "��������� ������", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("������", "��������� ������ �� �������", "OK");
        }
        return null;
    }
}
