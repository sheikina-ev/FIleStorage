using FIleStorage.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Net.Http;
using System.IO;
using Microsoft.Maui.Storage;
using FIleStorage.Utils;
using File = FIleStorage.Models.File;

namespace FIleStorage.Views
{
    public partial class FilesPage : ContentPage
    {
        private readonly UserService _userService;
        private readonly HttpClient _httpClient;
        private readonly string _token;

        public FilesPage()
        {
            InitializeComponent();
            _token = UserData.Token;
            _httpClient = new HttpClient();
            _userService = new UserService(_httpClient, _token);

            LoadUserFiles();
        }

        private async void LoadUserFiles()
        {
            try
            {
                var files = await _userService.GetUserFilesAsync();
                FilesCollectionView.ItemsSource = files;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ �������� ������: {ex.Message}");
                await DisplayAlert("������", "�� ������� ��������� �����.", "OK");
            }
        }

        private async void OnFileSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is File selectedFile)
            {
                await ShowFileDetails(selectedFile);
            }
        }

        private async Task ShowFileDetails(File file)
        {
            await DisplayAlert("���������� � �����",
                $"���: {file.Name}\n" +
                $"����������: {file.Extension}\n" +
                $"������: {file.Size} KB\n" +
                $"����: {file.Path}",
                "OK");
        }

        private async void OnDeleteFileClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is File file)
            {
                bool confirm = await DisplayAlert("�������� �����", $"�� �������, ��� ������ ������� {file.Name}?", "��", "���");
                if (confirm)
                {
                    await DeleteFile(file);
                }
            }
        }

        private async Task DeleteFile(File file)
        {
            try
            {
                bool success = await _userService.DeleteUserFileAsync(file.Id);
                if (success)
                {
                    await DisplayAlert("�����", "���� ������� ������", "OK");
                    LoadUserFiles();
                }
                else
                {
                    await DisplayAlert("������", "�� ������� ������� ����", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ �������� �����: {ex.Message}");
                await DisplayAlert("������", "��������� ������ ��� �������� �����", "OK");
            }
        }

        private async void OnDownloadFileClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is File file)
            {
                bool confirm = await DisplayAlert("������� ����", $"�� ������ ������� {file.Name}?", "��", "���");
                if (confirm)
                {
                    await DownloadFile(file);
                }
            }
        }

        private async Task DownloadFile(File file)
        {
            try
            {
                string downloadUrl = $"https://yourapi.com/files/download/{file.Id}";
                var request = new HttpRequestMessage(HttpMethod.Get, downloadUrl);
                request.Headers.Add("Authorization", $"Bearer {_token}");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();

                    // ���� � ����� �������� (Downloads)
                    string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                    string localFilePath = Path.Combine(downloadsPath, $"{file.Name}.{file.Extension}");

                    // ���������� ����
                    await System.IO.File.WriteAllBytesAsync(localFilePath, fileBytes);

                    await DisplayAlert("�����", $"���� �������: {localFilePath}", "OK");

                }
                else
                {
                    await DisplayAlert("������", "�� ������� ������� ����.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ �������� �����: {ex.Message}");
                await DisplayAlert("������", "��������� ������ ��� ���������� �����.", "OK");
            }
        }

        // ���������� ��� ������ �������� ������ �����
        private async void OnUploadFileClicked(object sender, EventArgs e)
        {
            try
            {
                // ��������� ������ ������ �����
                var filePickerResult = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images // ��� ����� ������ �����, ������� ������ ������������
                });

                if (filePickerResult != null)
                {
                    // �������� ���� � �����
                    var filePath = filePickerResult.FullPath;
                    var fileName = filePickerResult.FileName;

                    // ������ �������� ���� �� ������
                    await UploadFileToServer(filePath, fileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ ������ �����: {ex.Message}");
                await DisplayAlert("������", "��������� ������ ��� ������ �����.", "OK");
            }
        }

        private async Task UploadFileToServer(string filePath, string fileName)
        {
            try
            {
                var fileContent = new MultipartFormDataContent();
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath); // ������ �����
                fileContent.Add(new ByteArrayContent(fileBytes), "file", fileName);

                var uploadUrl = "http://course-project-4/api/files"; // ��� ����
                var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl)
                {
                    Content = fileContent
                };
                request.Headers.Add("Authorization", $"Bearer {_token}");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("�����", "���� ������� ��������!", "OK");
                    LoadUserFiles(); // ��������� ������ ������
                }
                else
                {
                    await DisplayAlert("������", "�� ������� ��������� ����.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ �������� �����: {ex.Message}");
                await DisplayAlert("������", "��������� ������ ��� �������� �����.", "OK");
            }
        }
    }
}
