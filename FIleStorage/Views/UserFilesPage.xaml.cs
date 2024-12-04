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

        // �������� ������ ��� ������� ������������
        LoadFiles(user.Id);
    }

    // ����� �������� ������ ������������
    private async void LoadFiles(int userId)
    {
        try
        {
            // �������� ����� ������������
            var files = await _fileService.GetUserFilesAsync(userId, _token);

            // ������� � ���������� � ���������
            UserFiles.Clear();
            foreach (var file in files)
            {
                UserFiles.Add(file); // ��������� ����� � ��������� ��� �����������
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("������", "�� ������� ��������� �����", "OK");
        }
    }
}


