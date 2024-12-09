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
        private readonly UserService _userService;

        public UserProfile(User user, string token)
        {
            InitializeComponent();
            _user = user;
            _token = token;
            _httpClient = new HttpClient();
            _userService = new UserService(_httpClient, _token);

            LoadUserProfile();
            LoadUserFiles(); // Загружаем файлы пользователя сразу при открытии профиля
        }

        private void LoadUserProfile()
        {
            // Загружаем данные пользователя
            UsernameLabel.Text = _user.Username;
            UsernameEntry.Text = _user.Username;
            EmailEntry.Text = _user.Email;
        }

        private async void LoadUserFiles()
        {
            try
            {
                var files = await _userService.GetUserFilesAsync();
                FilesListView.ItemsSource = files;
            }
            catch (Exception ex)
            {
                // Обработка ошибок (например, показать сообщение пользователю)
                await DisplayAlert("Error", ex.Message, "OK");
            }
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
