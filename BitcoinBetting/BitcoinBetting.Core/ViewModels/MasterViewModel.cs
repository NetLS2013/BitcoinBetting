using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BitcoinBetting.Core.Views;
using BitcoinBetting.Core.Views.MasterDetail;

namespace BitcoinBetting.Core.ViewModels
{
    class MasterViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<MenuItemPage> MenuItems { get; set; }
            
        public MasterViewModel()
        {
            MenuItems = new ObservableCollection<MenuItemPage>(new[]
            {
                new MenuItemPage { Id = 0, Title = "start up", TargetType = typeof(StartupPage)},
                new MenuItemPage { Id = 1, Title = "settings", TargetType = typeof(RegistrationPage)}
            });
        }
            
        public event PropertyChangedEventHandler PropertyChanged;
            
        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}