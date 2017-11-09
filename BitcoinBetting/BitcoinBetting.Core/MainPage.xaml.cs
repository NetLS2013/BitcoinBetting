using BitcoinBetting.Core.ViewModels;
using BitcoinBetting.Core.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BitcoinBetting.Core
{
    public partial class MainPage : ContentPage
    {
        public MainViewModel mainViewModel;

        private LoginPage loginPage { get; set; }

        private RegistrationPage registrationPage { get; set; }

        public MainPage()
        {
            loginPage = new LoginPage();
            registrationPage = new RegistrationPage();

            mainViewModel= new MainViewModel(this.Navigation, loginPage, registrationPage);
            InitializeComponent();
            this.BindingContext = mainViewModel;
        }
    }
}
