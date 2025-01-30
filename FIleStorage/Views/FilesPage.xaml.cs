using FIleStorage.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Net.Http;
using File = FIleStorage.Models.File;
using FIleStorage.Utils;

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
                $"������: {file.Size}\n" +
                $"����: {file.Path}\n" +
                $"���� ��������: {file.CreatedAt?.ToString("g") ?? "�� �������"}",
                "OK");
        }

        private async void OnDeleteFileClicked(object sender, EventArgs e)
        {
            // �������� ������ ����� �� BindingContext ������
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
    }
}
