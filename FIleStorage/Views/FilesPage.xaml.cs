using FIleStorage.Models;
using Newtonsoft.Json; // ��������� ���������� ��� ������ � JSON
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Net.Http.Headers;
using File = FIleStorage.Models.File;

namespace FIleStorage.Views
{
    public partial class FilesPage : ContentPage
    {
        private readonly User _user;
        private readonly string _token;
        private readonly HttpClient _httpClient;
        private readonly UserService _userService;

        public ObservableCollection<File> Files { get; set; } = new ObservableCollection<File>();

        public bool HasNoFiles => !Files.Any();

        public FilesPage()
        {
            InitializeComponent();
            _user = UserData.User; // �������� ������ � ������������ �� UserData
            _token = UserData.Token; // �������� �����

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

        // ����� ��� �������� �����
        public Command UploadFileCommand => new Command(async () =>
        {
            var filePickerResult = await FilePicker.PickAsync(); // ���������� FilePicker ��� ������ �����

            if (filePickerResult != null)
            {
                await UploadFileAsync(filePickerResult.FullPath); // ��������� ��������� ����
            }
        });

        // ����� �������� �����
        private async Task UploadFileAsync(string filePath)
        {
            try
            {
                var content = new MultipartFormDataContent();
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var fileName = Path.GetFileName(filePath);

                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = fileName
                };

                content.Add(fileContent);

                var request = new HttpRequestMessage(HttpMethod.Post, "https://yourapi.com/files"); // ������� ���������� URL
                request.Headers.Add("Authorization", $"Bearer {_token}");
                request.Content = content;

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseBody);
                    await DisplayAlert("Success", "File successfully uploaded.", "OK");
                    LoadUserFiles(); // ������������� ������ ������
                }
                else
                {
                    await DisplayAlert("Error", "Failed to upload the file.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while uploading the file: {ex.Message}", "OK");
            }
        }

        // �������������� ������, ����� ��� ����������, �������������� � �������� ������
    }
}
