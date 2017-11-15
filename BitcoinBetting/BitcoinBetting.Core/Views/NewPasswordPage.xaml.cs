using BitcoinBetting.Core.Models.User;
using BitcoinBetting.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BitcoinBetting.Core.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewPasswordPage : ContentPage
	{
        public NewPasswordPage (ForgotPasswordModel model)
		{
			InitializeComponent ();
            this.BindingContext = new NewPasswordViewModel(Navigation, model);
        }
    }
}