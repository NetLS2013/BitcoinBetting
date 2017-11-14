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
    public class RegistrationViewModel : BaseViewModel
    {
        public RegistrationModel registrationModel;

        public ICommand RegistrationCommand => new Command(async () => await Register());

        public ICommand FacebookLoginCommand => new Command(async () => await FacebookLogin());

        public ICommand GoogleLoginCommand => new Command(async () => await GoogleLogin());

        public IRequestProvider requestProvider { get; set; }

        public bool IsValid { get; set; }

        private ValidatableObject<string> email;
        private ValidatableObject<string> password;
        private ValidatableObject<string> repassword;
        private ValidatableObject<string> firstName;
        private ValidatableObject<string> lastName;

        public RegistrationViewModel()
        {
            requestProvider = new RequestProvider();
            registrationModel = new RegistrationModel();

            email = new ValidatableObject<string>();
            password = new ValidatableObject<string>();
            repassword = new ValidatableObject<string>();
            firstName = new ValidatableObject<string>();
            lastName = new ValidatableObject<string>();

            AddValidations();
        }

        public ValidatableObject<string> Email
        {
            get
            {
                registrationModel.Email = email.Value;
                return email;
            }
            set
            {
                if (email.Value != value.Value)
                {
                    email = value;
                    OnPropertyChanged("UserName");
                }
            }
        }

        public ValidatableObject<string> Password
        {
            get
            {
                registrationModel.Password = password.Value;
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

        public ValidatableObject<string> RePassword
        {
            get
            {
                return repassword;
            }
            set
            {
                if (repassword.Value != value.Value)
                {
                    repassword = value;
                    OnPropertyChanged("RePassword");
                }
            }
        }

        public ValidatableObject<string> FirstName
        {
            get
            {
                registrationModel.FirstName = firstName.Value;
                return firstName;
            }
            set
            {
                if (firstName.Value != value.Value)
                {
                    firstName = value;
                    OnPropertyChanged("FirstName");
                }
            }
        }

        public ValidatableObject<string> LastName
        {
            get
            {
                registrationModel.LastName = lastName.Value;
                return lastName;
            }
            set
            {
                if (lastName.Value != value.Value)
                {
                    lastName = value;
                    OnPropertyChanged("LastName");
                }
            }
        }

        private async Task Register()
        {
            IsBusy = true;
            IsValid = Validate();

            if (!IsValid)
            {
                string error = string.Empty;
                error += FirstName.Errors.Count > 0? FirstName.Errors[0] + Environment.NewLine : string.Empty;
                error += LastName.Errors.Count > 0 ? LastName.Errors[0] + Environment.NewLine : string.Empty;
                error += Email.Errors.Count > 0 ? Email.Errors[0] + Environment.NewLine : string.Empty;
                error += Password.Errors.Count > 0 ? Password.Errors[0] : string.Empty;

                await Application.Current.MainPage.DisplayAlert("Registration fail", error, "Ok");
            }
            else
            {
                try
                {
                    var result = await this.requestProvider.PostAsync<RegistrationModel, Result>(GlobalSetting.Instance.RegisterEndpoint, this.registrationModel);

                    if (!result.result)
                    {
                        await Application.Current.MainPage.DisplayAlert("Registration fail", result.Message, "Ok");
                    }
                    else
                    {
                        // some logic when login success
                    }
                }
                catch(Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("Registration fail", e.Message, "Ok");
                }
            }

            IsBusy = false;
        }

        private bool Validate()
        {
            var isValid = FirstName.Validate() && LastName.Validate() && Email.Validate() && FirstName.Validate() && Password.Validate();
            if(isValid && password.Value != repassword.Value)
            {
                Password.Errors.Clear();
                Password.Errors.Add("A password and re-password is not equals");
            }

            return isValid;
        }

        private void AddValidations()
        {
            FirstName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A first name is required" });

            LastName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A last name is required" });

            Email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A email is required" });

            Password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A password is required" });
        }

        private async Task FacebookLogin()
        {
            Device.OpenUri(new Uri(GlobalSetting.Instance.ExternalLoginFacebookEndpoint));
        }

        private async Task GoogleLogin()
        {
            Device.OpenUri(new Uri(GlobalSetting.Instance.ExternalLoginGoogleEndpoint));
        }
    }
}
