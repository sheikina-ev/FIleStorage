using FIleStorage.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Text;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using FIleStorage.Views.Auth;
using FIleStorage.Utils;

namespace FIleStorage.Views
{
    public partial class UserUpdateProfilePage : ContentPage
    {
        private readonly User _user;
        private readonly string _token;
        private readonly HttpClient _httpClient;

        public event Action<User> UserProfileUpdated; // ������� ��� ���������� �������

        public UserUpdateProfilePage()
        {
            InitializeComponent();
            _user = UserData.User;
            _token = UserData.Token;
            _httpClient = new HttpClient();

            LoadUserProfile();

            // ���������, ��� ����� �������� ���������� userId
            if (!ValidateToken(_token, _user.Id.ToString()))
            {
                Console.WriteLine("������: ����� �� �������� ����������� userId.");
            }
        }

        private void LoadUserProfile()
        {
            // ��������� ������ � ����
            NameEntry.Text = _user.Name;
            SurnameEntry.Text = _user.Surname;
            UsernameEntry.Text = _user.Username;
            UsernameLabel.Text = _user.Username;
            EmailEntry.Text = _user.Email;
            PhoneEntry.Text = string.IsNullOrEmpty(_user.Phone) ? "" : _user.Phone;
        }

        private async void OnUpdateButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // ��������� ������ _user ������ �������
                _user.Name = NameEntry.Text;
                _user.Surname = SurnameEntry.Text;
                _user.Username = UsernameEntry.Text;
                _user.Email = EmailEntry.Text;
                _user.Phone = PhoneEntry.Text;

                // �������� ������������ ������
                Console.WriteLine("������������ JSON:");
                Console.WriteLine(JsonSerializer.Serialize(_user));

                // ���������� ������
                var success = await UpdateUserProfileAsync();

                if (success)
                {
                    // ���� �������, ��������� UI � ������ �������
                    LoadUserProfile();  // ���������� UI � ������ �������

                    UserProfileUpdated?.Invoke(_user); // ������������� �� ����������
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
                // �������� ����������
                Console.WriteLine($"������: {ex.Message}");
                await DisplayAlert("������", $"��������� ������: {ex.Message}", "OK");
            }
        }

        private async Task<bool> UpdateUserProfileAsync()
        {
            try
            {
                // ������� ��������� ������ ��� �������� ������
                var updatePayload = new
                {
                    id = _user.Id,
                    name = _user.Name,
                    surname = _user.Surname,
                    email = _user.Email,
                    phone = _user.Phone,
                    username = _user.Username
                };

                // ������������ ������� � JSON
                var jsonContent = new StringContent(JsonSerializer.Serialize(updatePayload), Encoding.UTF8, "application/json");

                // ������������� ����� �����������
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                // ���������� ������
                var response = await _httpClient.PostAsync("http://course-project-4/api/profile", jsonContent);

                // ������ ����� �� �������
                var responseContent = await response.Content.ReadAsStringAsync();

                // �������� ������-��� � ����� �������
                Console.WriteLine("������-��� ������: " + response.StatusCode);
                Console.WriteLine("����� �������: " + responseContent);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                // ���� ������ ������ ������, ���������� ��
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent);
                ErrorLabel.Text = errorResponse?.Message ?? "����������� ������.";
                ErrorLabel.IsVisible = true;

                return false;
            }
            catch (Exception ex)
            {
                // �������� ����������
                Console.WriteLine($"������ ��� ���������� �������: {ex.Message}");
                return false;
            }
        }

        private async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            // ���������� ������������� ������
            bool answer = await DisplayAlert("�������������", "�� ������������� ������ �����?", "��", "���");
            if (answer)
            {
                // ���������� ������ ������������
                UserData.User = null;
                UserData.Token = null;

                // ��������� ������������� ����
                Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;

                // ������� �� �������� ������
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }


        private bool ValidateToken(string token, string expectedUserId)
        {
            try
            {
                // ��������� ����� �� ����� (header, payload, signature)
                var parts = token.Split('.');
                if (parts.Length != 3)
                {
                    Console.WriteLine("�������� ������ ������.");
                    return false;
                }

                // ���������� payload (������ �������)
                var payload = parts[1];
                var decodedPayload = Base64UrlDecode(payload);
                var payloadJson = Encoding.UTF8.GetString(decodedPayload);

                // ����������� payload � ������
                var payloadObject = JsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson);

                // ���������, ��� ����� �������� ��������� userId
                if (payloadObject.TryGetValue("sub", out var userId))
                {
                    if (userId.ToString() == expectedUserId)
                    {
                        return true;
                    }
                }

                Console.WriteLine("����� �� �������� ���������� userId.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ ��� �������� ������: {ex.Message}");
                return false;
            }
        }

        private byte[] Base64UrlDecode(string input)
        {
            string base64 = input.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }

    // ������ ������ ��� ��������� ������ �������
    public class ErrorResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
