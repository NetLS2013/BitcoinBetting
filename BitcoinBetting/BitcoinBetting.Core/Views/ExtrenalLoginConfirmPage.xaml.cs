using BitcoinBetting.Core.Models.User;
using BitcoinBetting.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BitcoinBetting.Core.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ExtrenalLoginConfirmPage : ContentPage
	{
		public ExtrenalLoginConfirmPage (ExternalLoginConfirmModel confirmModel)
		{
			InitializeComponent ();
            this.BindingContext = new ExternalLoginConfirmationViewModel(this.Navigation, confirmModel);
        }
	}
}