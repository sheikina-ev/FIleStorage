using FIleStorage.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using FIleStorage.Services;
using FIleStorage.Views.Auth;
namespace FIleStorage.Views;

public partial class UserFilesPage : ContentPage
{
    private User _user;
    private string _token;
    private FileService _fileService;
    public ObservableCollection<FileModel> UserFiles { get; set; }

    public UserFilesPage(User user, string token)
    {
        InitializeComponent();
        _user = user;
        _token = token;
        _fileService = new FileService();
        UserFiles = new ObservableCollection<FileModel>();
        BindingContext = this;

        // Загрузка файлов для данного пользователя
        LoadFiles(user.Id);
    }

    // Метод загрузки файлов пользователя
    private async void LoadFiles(int userId)
    {
        try
        {
            // Получаем файлы пользователя
            var files = await _fileService.GetUserFilesAsync(userId, _token);

            // Очистка и добавление в коллекцию
            UserFiles.Clear();
            foreach (var file in files)
            {
                UserFiles.Add(file); // Добавляем файлы в коллекцию для отображения
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", "Не удалось загрузить файлы", "OK");
        }
    }
}


