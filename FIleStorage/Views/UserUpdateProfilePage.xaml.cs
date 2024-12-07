using FIleStorage.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Text;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace FIleStorage.Views
{
    public partial class UserUpdateProfilePage : ContentPage
    {
        private readonly User _user;
        private readonly string _token;
        private readonly HttpClient _httpClient;

        public event Action<User> UserProfileUpdated; // ������� ��� ���������� �������

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
                    // ��������� UI � ������ ���������� � _user
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
    }

    // ������ ������ ��� ��������� ������ �������
    public class ErrorResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
