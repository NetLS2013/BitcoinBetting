using BitcoinBetting.Core.ViewModels.Base;
using BitcoinBetting.Core.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BitcoinBetting.Core.Views.Account;
using Xamarin.Forms;

namespace BitcoinBetting.Core.ViewModels
{
    public class StartupViewModel : BaseViewModel
    {
        public ICommand LoginCommnad { set; get; }
        public ICommand RegisterCommnad { set; get; }
        
        private INavigation Navigation { set; get; }

        private readonly LoginPage loginPage;
        private readonly RegistrationPage registrationPage;

        public StartupViewModel(INavigation Navigation)
        {
            this.Navigation = Navigation;

            loginPage = new LoginPage();
            registrationPage = new RegistrationPage();
            
            LoginCommnad = new Command(LoadLoginPage);
            RegisterCommnad = new Command(LoadRegistrationPage);
        }

        private async void LoadLoginPage()
        {
            await Navigation.PushAsync(loginPage);
        }

        private async void LoadRegistrationPage()
        {
            await Navigation.PushAsync(registrationPage);
        }
    }
}
