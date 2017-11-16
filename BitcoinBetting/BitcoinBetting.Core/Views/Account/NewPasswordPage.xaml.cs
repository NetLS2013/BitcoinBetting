using BitcoinBetting.Core.Models.User;
using BitcoinBetting.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BitcoinBetting.Core.Views.Account
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewPasswordPage : ContentPage
	{
        public NewPasswordPage (ForgotPasswordModel model)
		{
			InitializeComponent ();
		    
            BindingContext = new NewPasswordViewModel(Navigation, model);
        }
    }
}