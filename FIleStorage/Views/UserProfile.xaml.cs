using FIleStorage.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using FIleStorage.Views;

namespace FIleStorage.Views
{
    public partial class UserProfile : ContentPage
    {
        private User _user;
        private readonly string _token;
        private readonly HttpClient _httpClient;

        // Конструктор, который принимает токен
        public UserProfile(User user, string token)
        {
            InitializeComponent();
            _user = user;
            _token = token;
            _httpClient = new HttpClient();

            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            // Загружаем данные пользователя
            UsernameLabel.Text = _user.Username;
            UsernameEntry.Text = _user.Username;
            EmailEntry.Text = _user.Email;
        }

        // Метод для обработки нажатия кнопки "Изменить"
        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            // Переход на страницу обновления профиля с передачей данных пользователя
            var updateProfilePage = new UserUpdateProfilePage(_user, _token);

            // Слушаем изменения профиля, и обновляем данные после возвращения
            updateProfilePage.UserProfileUpdated += OnUserProfileUpdated;

            await Navigation.PushAsync(updateProfilePage);
        }

        private void OnUserProfileUpdated(User updatedUser)
        {
            // Обновляем локальные данные
            _user = updatedUser;

            // Перезагружаем отображение данных на этой странице
            LoadUserProfile();
        }

    }
}
