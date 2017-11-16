using BitcoinBetting.Core.Models.User;
using BitcoinBetting.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BitcoinBetting.Core.Views.Account
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ExtrenalLoginConfirmPage : ContentPage
	{
		public ExtrenalLoginConfirmPage (ExternalLoginConfirmModel confirmModel)
		{
			InitializeComponent ();
			    
            BindingContext = new ExternalLoginConfirmationViewModel(Navigation, confirmModel);
        }
	}
}