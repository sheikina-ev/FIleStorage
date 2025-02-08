using FIleStorage.Models;
using Microsoft.Maui.Controls;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using FIleStorage.Utils;

namespace FIleStorage.Views
{
    public partial class PermissionsPage : ContentPage
    {
        private readonly string _token;
        private readonly HttpClient _httpClient;

        // ������ ��� ��������������� � ���������� ����
        public List<AccessRight> PermsGiven { get; set; } = new List<AccessRight>();
        public List<AccessRight> PermsReceived { get; set; } = new List<AccessRight>();

        public PermissionsPage()
        {
            InitializeComponent();

            // ������������� ������ ������������
            _token = UserData.Token;

            // ��������� HttpClient
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            // ������������� �������� ������
            BindingContext = this;

            // ��������� ������ � ������ �������
            LoadPermissions();
        }

        // �������� ��������������� � ���������� ����
        private async Task LoadPermissions()
        {
            await GetProvidedPermissions();
            await GetReceivedPermissions();
        }

        // ��������� ��������������� ����
        private async Task GetProvidedPermissions()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://course-project-4/api/perms");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var permissions = JsonSerializer.Deserialize<List<AccessRight>>(jsonResponse);
                    PermsGiven = permissions;
                    OnPropertyChanged(nameof(PermsGiven)); // ����������� ��������

                    Debug.WriteLine($"PermsGiven Loaded: {permissions.Count} items.");
                }
                else
                {
                    Debug.WriteLine("Error loading provided permissions.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                await DisplayAlert("������", "������ ��� �������� ��������������� ���� �������.", "OK");
            }
        }

        // ��������� ���������� ����
        private async Task GetReceivedPermissions()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://course-project-4/api/perms/my");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var permissions = JsonSerializer.Deserialize<List<AccessRight>>(jsonResponse);
                    PermsReceived = permissions;
                    OnPropertyChanged(nameof(PermsReceived)); // ����������� ��������

                    Debug.WriteLine($"PermsReceived Loaded: {permissions.Count} items.");
                }
                else
                {
                    Debug.WriteLine("Error loading received permissions.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                await DisplayAlert("������", "������ ��� �������� ���������� ���� �������.", "OK");
            }
        }

        // ����� ��� ���������� ������� � �����
        private async void OnAddAccessClicked(object sender, EventArgs e)
        {
            try
            {
                var username = UsernameEntry.Text?.Trim();
                var fileIdText = FileIdEntry.Text?.Trim();

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(fileIdText) || !long.TryParse(fileIdText, out var fileId))
                {
                    await DisplayAlert("������", "������� ���������� ������ ��� �������� � ID �����.", "OK");
                    return;
                }

                var requestBody = new { username, file_id = fileId };
                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"http://course-project-4/api/files/{fileId}/accesses", content);

                await HandleResponse(response, "������ ������������");
            }
            catch (Exception ex)
            {
                await DisplayAlert("������", $"��������� ������: {ex.Message}", "OK");
            }
        }

        // ����� ��� �������� ������� � �����
        private async void OnRemoveAccessClicked(object sender, EventArgs e)
        {
            try
            {
                var username = UsernameEntry.Text?.Trim();
                var fileIdText = FileIdEntry.Text?.Trim();

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(fileIdText) || !long.TryParse(fileIdText, out var fileId))
                {
                    await DisplayAlert("������", "������� ���������� ������ ��� �������� � ID �����.", "OK");
                    return;
                }

                var requestBody = new { username, file_id = fileId };
                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Delete, $"http://course-project-4/api/files/{fileId}/accesses")
                {
                    Content = content
                };

                var response = await _httpClient.SendAsync(request);

                await HandleResponse(response, "������ �����");
            }
            catch (Exception ex)
            {
                await DisplayAlert("������", $"��������� ������: {ex.Message}", "OK");
            }
        }

        // ������������� ����� ��� ��������� ������ �� API
        private async Task HandleResponse(HttpResponseMessage response, string successMessage)
        {
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);

                await DisplayAlert("�����", $"{successMessage}:\n{result["message"]}", "OK");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                string errorMessage;
                try
                {
                    var errorData = JsonSerializer.Deserialize<Dictionary<string, object>>(errorContent);
                    errorMessage = errorData?["error"]?.ToString() ?? "����������� ������.";
                }
                catch
                {
                    errorMessage = "������ ��������� ������ �������.";
                }

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        errorMessage = "���� � ��������� ID �� ������.";
                        break;
                    case System.Net.HttpStatusCode.Unauthorized:
                        errorMessage = "�� �� ������������. ����������, ������� � �������.";
                        break;
                }

                await DisplayAlert("������", errorMessage, "OK");
            }
        }
    }
}
