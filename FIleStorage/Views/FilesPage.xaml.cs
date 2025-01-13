using FIleStorage.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

// ��������� ��������� ��� ������ File
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

            BindingContext = this; // ������������� �������� ������
            LoadUserFiles();       // ��������� ����� ������������
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

                OnPropertyChanged(nameof(HasNoFiles)); // ���������� �� ��������� ��������
            }
            catch (Exception ex)
            {
                await DisplayAlert("������", $"��������� ������ ��� �������� ������: {ex.Message}", "OK");
            }
        }

        public Command FileSelectedCommand => new Command<MyFile>(async (file) =>
        {
            if (file != null)
            {
                await DisplayAlert("���������� � �����",
                    $"���: {file.Name}\n" +
                    $"����������: {file.Extension}\n" +
                    $"������: {file.Size} KB\n" +
                    $"����: {file.Path}\n" +
                    $"���� ��������: {file.CreatedAt?.ToString("g") ?? "�� �������"}",
                    "OK");
            }
        });
    }
}
