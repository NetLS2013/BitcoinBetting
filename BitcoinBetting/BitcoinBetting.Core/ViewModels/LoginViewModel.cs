using BitcoinBetting.Core.Interfaces;
using BitcoinBetting.Core.Models.User;
using BitcoinBetting.Core.Services;
using BitcoinBetting.Core.Services.Validations;
using BitcoinBetting.Core.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BitcoinBetting.Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public LoginModel loginModel;

        public ICommand LoginCommand => new Command(async () => await Login());

        public IRequestProvider requestProvider { get; set; }

        public bool IsValid { get; set; }

        private ValidatableObject<string> email;
        private ValidatableObject<string> password;
        private ValidatableObject<bool> isRemember;

        public LoginViewModel()
        {
            requestProvider = new RequestProvider();
            loginModel = new LoginModel();

            email = new ValidatableObject<string>();
            password = new ValidatableObject<string>();
            isRemember = new ValidatableObject<bool>();

            AddValidations();
        }

        public ValidatableObject<string> Email
        {
            get
            {
                loginModel.Email = email.Value;
                return email;
            }
            set
            {
                if (email.Value != value.Value)
                {
                    email = value;
                    OnPropertyChanged("Email");
                }
            }
        }

        public ValidatableObject<string> Password
        {
            get
            {
                loginModel.Password = password.Value;
                return password;
            }
            set
            {
                if (password.Value != value.Value)
                {
                    password = value;
                    OnPropertyChanged("Password");
                }
            }
        }

        public ValidatableObject<bool> IsRemember
        {
            get
            {
                return isRemember;
            }
            set
            {
                if (isRemember.Value != value.Value)
                {
                    isRemember = value;
                    OnPropertyChanged("IsRemember");
                }
            }
        }

        private async Task Login()
        {
            IsBusy = true;
            IsValid = Validate();

            if (!IsValid)
            {
                string error = string.Empty;
                error += Email.Errors.Count > 0 ? Email.Errors[0] + Environment.NewLine : string.Empty;
                error += Password.Errors.Count > 0 ? Password.Errors[0] : string.Empty;

                await Application.Current.MainPage.DisplayAlert("Login fail", error, "Ok");
            }
            else
            {
                try
                {
                    var result = await this.requestProvider.PostAsync<LoginModel, Result>(GlobalSetting.Instance.LoginEndpoint, this.loginModel);

                    if (!result.result)
                    {
                        await Application.Current.MainPage.DisplayAlert("Login fail", result.Message, "Ok");
                    }
                    else
                    {
                        // some logic when login success
                    }
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("Login fail", e.Message, "Ok");
                }
            }

            IsBusy = false;
        }

        private bool Validate()
        {
            return Email.Validate() && Password.Validate();
        }

        private void AddValidations()
        {
            Email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A email is required" });

            Password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A password is required" });
        }
    }
}
