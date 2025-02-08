using FIleStorage.Views.Auth;
namespace FIleStorage.Views

{
    public partial class MainPage : ContentPage
    {
     
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnStartButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");

        }
    }

}
