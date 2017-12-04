using BitcoinBetting.Core.Interfaces;
using BitcoinBetting.Core.Models.User;
using BitcoinBetting.Core.Services;
using BitcoinBetting.Core.Services.Validations;
using BitcoinBetting.Core.ViewModels.Base;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BitcoinBetting.Core.Models;
using BitcoinBetting.Core.Views;
using BitcoinBetting.Core.Views.Account;
using BitcoinBetting.Core.Views.Menu;
using Xamarin.Forms;

namespace BitcoinBetting.Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public LoginModel loginModel;

        public ICommand LoginCommand => new Command(async () => await Login());
        public ICommand FacebookLoginCommand => new Command(async () => await FacebookLogin());
        public ICommand GoogleLoginCommand => new Command(async () => await GoogleLogin());
        public ICommand ForgotPasswordCommand => new Command(async () => await ForgotPassword());

        public IRequestProvider requestProvider { get; set; }

        private bool IsValid { get; set; }
        private INavigation Navigation { get; set;}
        private Page CurrentPage { get; set;}
        
        private ValidatableObject<string> email;
        private ValidatableObject<string> password;
        private ValidatableObject<bool> isRemember;
        
        public LoginViewModel(INavigation navigation, Page currentPage)
        {
            this.Navigation = navigation;
            this.CurrentPage = currentPage;
            
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
                error += Email.Errors.Count > 0 ? Environment.NewLine + Email.Errors[0] : string.Empty;
                error += Password.Errors.Count > 0 ? Environment.NewLine + Password.Errors[0] : string.Empty;

                await Application.Current.MainPage.DisplayAlert("Login fail", error, "Ok");
            }
            else
            {
                try
                {
                    var result = await requestProvider.PostAsync<LoginModel, Result>(GlobalSetting.Instance.LoginEndpoint, this.loginModel);

                    if (!result.result)
                    {
                        await Application.Current.MainPage.DisplayAlert("Login fail", result.Message, "Ok");
                    }
                    else
                    {
                        Application.Current.Properties["token"] = result.token;
                        Application.Current.Properties["refresh_token"] = result.refresh_token;

                        Application.Current.MainPage = new MasterPage();
                    }
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("Login fail", e.Message, "Ok");
                }
            }

            IsBusy = false;
        }
        
        private async Task ForgotPassword()
        {
            await Navigation.PushAsync(new ForgotPasswordPage());
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

        private async Task FacebookLogin()
        {
            Device.OpenUri(new Uri(GlobalSetting.Instance.ExternalLoginFacebookEndpoint));
            
            CloseAndroidApp();
        }

        private async Task GoogleLogin()
        {
            Device.OpenUri(new Uri(GlobalSetting.Instance.ExternalLoginGoogleEndpoint));
            
            CloseAndroidApp();
        }

        private void CloseAndroidApp()
        {
#if (!DEBUG)
            if (Device.RuntimePlatform == Device.Android)
            {
                DependencyService.Get<INativeHelpers>().CloseApp();
            }
#endif
        }
    }
}
