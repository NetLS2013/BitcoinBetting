using BitcoinBetting.Core.ViewModels.Base;
using BitcoinBetting.Core.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BitcoinBetting.Core.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ICommand LoginCommnad { protected set; get; }

        public ICommand RegisterCommnad { protected set; get; }

        public INavigation Navigation { protected set; get; }

        private LoginPage loginPage;

        private RegistrationPage registrationPage;

        public MainViewModel(INavigation Navigation, LoginPage loginPage, RegistrationPage registrationPage)
        {
            this.Navigation = Navigation;
            LoginCommnad = new Command(LoadLoginPage);
            RegisterCommnad = new Command(LoadRegistrationPage);

            this.loginPage = loginPage;
            this.registrationPage = registrationPage;
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
