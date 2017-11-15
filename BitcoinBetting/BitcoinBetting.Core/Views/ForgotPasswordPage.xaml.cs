using BitcoinBetting.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BitcoinBetting.Core.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ForgotPasswordPage : ContentPage
	{
        public ForgotPasswordPage ()
		{
			InitializeComponent ();
            this.BindingContext = new ForgotPasswordViewModel(Navigation, this);
        }
    }
}