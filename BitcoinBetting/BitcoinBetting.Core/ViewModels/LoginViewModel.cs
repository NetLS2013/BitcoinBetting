using BitcoinBetting.Models.User;
using BitcoinBetting.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BitcoinBetting.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public LoginModel loginModel;

        public ICommand LoginCommand { protected set; get; }

        public LoginViewModel()
        {
            loginModel = new LoginModel();
        }

        public string UserName
        {
            get { return loginModel.Email; }
            set
            {
                if (loginModel.Email != value)
                {
                    loginModel.Email = value;
                    OnPropertyChanged("UserName");
                }
            }
        }

        public string Password
        {
            get { return loginModel.Password; }
            set
            {
                if (loginModel.Password != value)
                {
                    loginModel.Password = value;
                    OnPropertyChanged("Password");
                }
            }
        }

        public bool IsRemember
        {
            get { return loginModel.IsRemember; }
            set
            {
                if (loginModel.IsRemember != value)
                {
                    loginModel.IsRemember = value;
                    OnPropertyChanged("IsRemember");
                }
            }
        }
    }
}
