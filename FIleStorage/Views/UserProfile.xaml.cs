using FIleStorage.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using FIleStorage.Views;

// Добавляем псевдоним для вашего класса File
using MyFile = FIleStorage.Models.File;

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

                // Добавляем обработчик клика по файлам
                FilesListView.ItemTapped += OnFileTapped;
            }
            catch (Exception ex)
            {
                // Обработка ошибок (например, показать сообщение пользователю)
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnFileTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is MyFile selectedFile)
            {
                await DisplayAlert("Информация о файле",
                    $"Имя: {selectedFile.Name}\n" +
                    $"Расширение: {selectedFile.Extension}\n" +
                    $"Размер: {selectedFile.Size}\n" +
                    $"Путь: {selectedFile.Path}\n" +
                    $"Дата создания: {selectedFile.CreatedAt?.ToString("g") ?? "Не указана"}",
                    "OK");
            }

            // Сбрасываем выделение элемента
            ((ListView)sender).SelectedItem = null;
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
