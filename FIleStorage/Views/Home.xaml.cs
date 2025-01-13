using FIleStorage.Models;
using FIleStorage.Utils;
using FIleStorage.Views.Auth;

namespace FIleStorage.Views;

public partial class Home : ContentPage
{
    private User _user;
    private string _token;
    public Home()
    {
        InitializeComponent();
        _user = UserData.User;
        _token = UserData.Token;
    }
}