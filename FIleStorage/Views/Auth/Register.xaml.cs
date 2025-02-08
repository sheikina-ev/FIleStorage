using FIleStorage.Models;
using FIleStorage.Utils;
using System.Net.Http.Json;
using System.Text.Json;

namespace FIleStorage.Views.Auth
{
    public partial class Register : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public Register()
        {
            InitializeComponent();
        }

        // ������� �� �������� �����
        private async void OnLoginTapped(object sender, EventArgs e)
        {
            // ������� �� �������� �����
            await Shell.Current.GoToAsync("//LoginPage");

        }

        // �����������
        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            // ���� ������ �� �����
            string surname = SurnameEntry.Text;
            string name = NameEntry.Text;
            string username = UsernameEntry.Text;
            string email = EmailEntry.Text;
            string password = PasswordEntry.Text;
            string phone = PhoneEntry.Text;

            // �������� �� ������ ����
            if (string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("������", "Surname, Name, Username, Email � Password ������ ���� ���������", "OK");
                return;
            }

            // ������������ ���� �������
            var registerData = new MultipartFormDataContent
    {
        { new StringContent(surname), "surname" },
        { new StringContent(name), "name" },
        { new StringContent(username), "username" },
        { new StringContent(email), "email" },
        { new StringContent(password), "password" }
    };

            // ��������� �������������� ����, ���� ��� ���������
            if (!string.IsNullOrWhiteSpace(phone))
            {
                registerData.Add(new StringContent(phone), "phone");
            }

            try
            {
                // ���������� ������ � ���������� ����� � response
                HttpResponseMessage response = await _httpClient.PostAsync("http://course-project-4/api/register", registerData);

                if (response.IsSuccessStatusCode)
                {
                    // �������� �����������
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<AuthResponse>(content);

                    if (result?.Token != null)
                    {
                        await DisplayAlert("�������� �����������", "����������� ��������� �������. ����� ����������!", "��");
                        
                        UserData.User = result.User;
                        UserData.Token = result.Token;
                        
                        await Navigation.PushAsync(new UserProfile());
                    }
                    else
                    {
                        await DisplayAlert("������", "�� ������� �������� ������ ��� �����. ����������, ���������� �����.", "��");
                    }
                }
                else
                {
                    // ��������� ������ � ������� (��������, ���������)
                    var content = await response.Content.ReadAsStringAsync();
                    var errorMessage = ExtractErrorMessage(content);

                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        errorMessage = "��������� ������ �� �������";
                    }

                    await DisplayAlert("������", errorMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                // ����� ���������� � ���������� ������ ����
                await DisplayAlert("������ ����", ex.Message, "��");
            }
        }

        // ����� ��� ���������� ������ �� ������ �������
        private string ExtractErrorMessage(string content)
        {
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content);
                return errorResponse?.Message ?? "����������� ������";
            }
            catch
            {
                return "����������� ������";
            }
        }
    }

    // ������ ��� ������ �� �������
    public class ErrorResponse
    {
        public string Message { get; set; }
    }
}
