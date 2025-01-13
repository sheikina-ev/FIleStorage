using FIleStorage.Utils;
using Microsoft.Maui.Controls;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FIleStorage.Views
{
    public partial class PermissionsPage : ContentPage
    {
        private readonly string _token;
        private readonly HttpClient _httpClient;

        public PermissionsPage()
        {
            InitializeComponent();
            _token = UserData.Token;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
        }

        private async void OnAddAccessClicked(object sender, EventArgs e)
        {
            try
            {
                var username = UsernameEntry.Text?.Trim();
                var fileIdText = FileIdEntry.Text?.Trim();

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(fileIdText) || !long.TryParse(fileIdText, out var fileId))
                {
                    await DisplayAlert("Ошибка", "Введите корректные данные для никнейма и ID файла.", "OK");
                    return;
                }

                var requestBody = new { username, file_id = fileId };
                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"http://course-project-4/api/files/{fileId}/accesses", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);

                    await DisplayAlert("Успех", $"Доступ предоставлен:\n{result["message"]}", "OK");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    // Попытка обработки JSON для получения понятного сообщения
                    string errorMessage;
                    try
                    {
                        var errorData = JsonSerializer.Deserialize<Dictionary<string, object>>(errorContent);
                        if (errorData != null && errorData.ContainsKey("error"))
                        {
                            errorMessage = errorData["error"]?.ToString() ?? "Неизвестная ошибка.";
                        }
                        else
                        {
                            errorMessage = "Проверьте указанные данные!";
                        }
                    }
                    catch
                    {
                        errorMessage = "Произошла ошибка при попытке обработки ответа от сервера.";
                    }

                    // Вывод понятного сообщения на основе кода статуса
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.NotFound:
                            await DisplayAlert("Ошибка", "Файл с указанным ID не найден. Проверьте введённый ID.", "OK");
                            break;

                        case System.Net.HttpStatusCode.BadRequest:
                            await DisplayAlert("Ошибка", $"Некорректный запрос: {errorMessage}", "OK");
                            break;

                        case System.Net.HttpStatusCode.Unauthorized:
                            await DisplayAlert("Ошибка", "Вы не авторизованы. Пожалуйста, войдите в систему.", "OK");
                            break;

                        case System.Net.HttpStatusCode.Forbidden:
                            await DisplayAlert("Ошибка", "Файл с указанным ID не найден.", "OK");
                            break;

                        default:
                            await DisplayAlert("Ошибка", $"Произошла ошибка: {errorMessage}", "OK");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "OK");
            }
        }


        private async void OnRemoveAccessClicked(object sender, EventArgs e)
        {
            try
            {
                var username = UsernameEntry.Text?.Trim();
                var fileIdText = FileIdEntry.Text?.Trim();

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(fileIdText) || !long.TryParse(fileIdText, out var fileId))
                {
                    await DisplayAlert("Ошибка", "Введите корректные данные для никнейма и ID файла.", "OK");
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

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);

                    await DisplayAlert("Успех", $"Доступ удалён:\n{result["message"]}", "OK");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Ошибка", $"Проверьте указанные данные!", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "OK");
            }
        }
    }
}
