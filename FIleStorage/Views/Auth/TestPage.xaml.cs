using FIleStorage.Models;
namespace FIleStorage.Views.Auth;



public partial class TestPage : ContentPage
{
	public TestPage()
	{
		InitializeComponent();
	}
    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Login());
    }

    private async void OnRegisterButtonClicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new Register());
    }
  
}