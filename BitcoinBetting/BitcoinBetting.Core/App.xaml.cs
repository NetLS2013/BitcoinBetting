using System.Linq;
using Xamarin.Forms;

namespace BitcoinBetting.Core
{
    public partial class App : Application
    {
        public App(Page mainPage)
        {
            InitializeComponent();

            MainPage = mainPage;
        }

        protected override void OnStart()
        {
            
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
