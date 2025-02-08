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

        // Переход на страницу входа
        private async void OnLoginTapped(object sender, EventArgs e)
        {
            // Переход на страницу входа
            await Shell.Current.GoToAsync("//LoginPage");

        }

        // Регистрация
        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            // Сбор данных из формы
            string surname = SurnameEntry.Text;
            string name = NameEntry.Text;
            string username = UsernameEntry.Text;
            string email = EmailEntry.Text;
            string password = PasswordEntry.Text;
            string phone = PhoneEntry.Text;

            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Ошибка", "Surname, Name, Username, Email и Password должны быть заполнены", "OK");
                return;
            }

            // Формирование тела запроса
            var registerData = new MultipartFormDataContent
    {
        { new StringContent(surname), "surname" },
        { new StringContent(name), "name" },
        { new StringContent(username), "username" },
        { new StringContent(email), "email" },
        { new StringContent(password), "password" }
    };

            // Добавляем необязательное поле, если оно заполнено
            if (!string.IsNullOrWhiteSpace(phone))
            {
                registerData.Add(new StringContent(phone), "phone");
            }

            try
            {
                // Отправляем запрос и записываем ответ в response
                HttpResponseMessage response = await _httpClient.PostAsync("http://course-project-4/api/register", registerData);

                if (response.IsSuccessStatusCode)
                {
                    // Успешная регистрация
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<AuthResponse>(content);

                    if (result?.Token != null)
                    {
                        await DisplayAlert("Успешная регистрация", "Регистрация завершена успешно. Добро пожаловать!", "ОК");
                        
                        UserData.User = result.User;
                        UserData.Token = result.Token;
                        
                        await Navigation.PushAsync(new UserProfile());
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Не удалось получить данные для входа. Пожалуйста, попробуйте снова.", "ОК");
                    }
                }
                else
                {
                    // Обработка ошибок с сервера (например, валидации)
                    var content = await response.Content.ReadAsStringAsync();
                    var errorMessage = ExtractErrorMessage(content);

                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        errorMessage = "Произошла ошибка на сервере";
                    }

                    await DisplayAlert("Ошибка", errorMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                // Ловим исключения и показываем ошибку сети
                await DisplayAlert("Ошибка сети", ex.Message, "ОК");
            }
        }

        // Метод для извлечения ошибки из ответа сервера
        private string ExtractErrorMessage(string content)
        {
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content);
                return errorResponse?.Message ?? "Неизвестная ошибка";
            }
            catch
            {
                return "Неизвестная ошибка";
            }
        }
    }

    // Модель для ошибки от сервера
    public class ErrorResponse
    {
        public string Message { get; set; }
    }
}
