using FIleStorage.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

// Добавляем псевдоним для класса File
using MyFile = FIleStorage.Models.File;
using FIleStorage.Utils;

namespace FIleStorage.Views
{
    public partial class FilesPage : ContentPage
    {
        private readonly User _user;
        private readonly string _token;
        private readonly HttpClient _httpClient;
        private readonly UserService _userService;

        public ObservableCollection<MyFile> Files { get; set; } = new ObservableCollection<MyFile>();

        public bool HasNoFiles => !Files.Any();

        public FilesPage()
        {
            InitializeComponent();
            _user = UserData.User;
            _token = UserData.Token;

            _httpClient = new HttpClient();
            _userService = new UserService(_httpClient, _token);

            BindingContext = this; // Устанавливаем контекст данных
            LoadUserFiles();       // Загружаем файлы пользователя
        }

        private async void LoadUserFiles()
        {
            try
            {
                var files = await _userService.GetUserFilesAsync();

                Files.Clear();
                foreach (var file in files)
                {
                    Files.Add(file);
                }

                OnPropertyChanged(nameof(HasNoFiles)); // Уведомляем об изменении свойства
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка при загрузке файлов: {ex.Message}", "OK");
            }
        }

        public Command FileSelectedCommand => new Command<MyFile>(async (file) =>
        {
            if (file != null)
            {
                await DisplayAlert("Информация о файле",
                    $"Имя: {file.Name}\n" +
                    $"Расширение: {file.Extension}\n" +
                    $"Размер: {file.Size} KB\n" +
                    $"Путь: {file.Path}\n" +
                    $"Дата создания: {file.CreatedAt?.ToString("g") ?? "Не указана"}",
                    "OK");
            }
        });
    }
}
