using FIleStorage.Models;
using FIleStorage.Views.Auth;

namespace FIleStorage.Views;

public partial class Home : ContentPage
{
    private User _user;
    private string _token;
    public Home(User user, string token)
    {
        InitializeComponent();
        _user = user;
        _token = token;


    }

 
}