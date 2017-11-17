using BitcoinBetting.Core.ViewModels;
using Xamarin.Forms;

namespace BitcoinBetting.Core
{
    public partial class StartupPage : ContentPage
    {
        public StartupPage()
        {
            InitializeComponent();
            
            BindingContext = new StartupViewModel(Navigation);
        }
    }
}
