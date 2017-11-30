using BitcoinBetting.Core.Interfaces;
using BitcoinBetting.Core.Models.User;
using BitcoinBetting.Core.Services;
using BitcoinBetting.Core.Services.Validations;
using BitcoinBetting.Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BitcoinBetting.Core.Models;
using BitcoinBetting.Core.Views.Menu;
using Xamarin.Forms;

namespace BitcoinBetting.Core.ViewModels
{
    public class ExternalLoginConfirmationViewModel : BaseViewModel
    {
        INavigation navigation;
        public ExternalLoginConfirmModel registrationModel;

        public ICommand ConfirmCommand => new Command(async () => await Confirm());

        public IRequestProvider requestProvider { get; set; }

        public bool IsValid { get; set; }

        private ValidatableObject<string> email;
        private ValidatableObject<string> firstName;
        private ValidatableObject<string> lastName;

        public ExternalLoginConfirmationViewModel(INavigation navigation, ExternalLoginConfirmModel externalLogin)
        {
            this.navigation = navigation;
            registrationModel = externalLogin;

            requestProvider = new RequestProvider();
            
            email = new ValidatableObject<string>() { Value = externalLogin.Email};
            firstName = new ValidatableObject<string>() { Value = externalLogin.FirstName };
            lastName = new ValidatableObject<string>() { Value = externalLogin.LastName };

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
                    OnPropertyChanged("Email");
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

        private async Task Confirm()
        {
            IsBusy = true;
            IsValid = Validate();

            if (!IsValid)
            {
                string error = string.Empty;
                error += FirstName.Errors.Count > 0 ? FirstName.Errors[0] + Environment.NewLine : string.Empty;
                error += LastName.Errors.Count > 0 ? LastName.Errors[0] + Environment.NewLine : string.Empty;
                error += Email.Errors.Count > 0 ? Email.Errors[0] + Environment.NewLine : string.Empty;

                await Application.Current.MainPage.DisplayAlert("Confirm fail", error, "Ok");
            }
            else
            {
                try
                {
                    var result = await this.requestProvider.PostAsync<ExternalLoginConfirmModel, Result>(GlobalSetting.Instance.ExternalLoginConfirmationEndpoint, this.registrationModel, 
                        new List<KeyValuePair<string, string>>() {
                            new KeyValuePair<string, string>( "Identity.External", registrationModel.Cookie)
                        });

                    if (!result.result)
                    {
                        await Application.Current.MainPage.DisplayAlert("Confirm fail", result.Message, "Ok");
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
                    await Application.Current.MainPage.DisplayAlert("Confirm fail", e.Message, "Ok");
                }
            }

            IsBusy = false;
        }

        private bool Validate()
        {
            return FirstName.Validate() && LastName.Validate() && Email.Validate() && FirstName.Validate();
        }

        private void AddValidations()
        {
            FirstName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A first name is required" });

            LastName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A last name is required" });

            Email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A email is required" });
        }
    }
}
