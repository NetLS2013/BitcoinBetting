using BitcoinBetting.Core.Interfaces;
using BitcoinBetting.Core.Models.User;
using BitcoinBetting.Core.Services;
using BitcoinBetting.Core.Services.Validations;
using BitcoinBetting.Core.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using BitcoinBetting.Core.Models;
using BitcoinBetting.Core.Views;
using BitcoinBetting.Core.Views.Account;
using Xamarin.Forms;

namespace BitcoinBetting.Core.ViewModels
{
    public class NewPasswordViewModel : BaseViewModel
    {
        private ForgotPasswordConfirmModel forgotPasswordConfirmModel;

        public ICommand ChangePasswordCommand => new Command(async () => await ChangePassword());

        public IRequestProvider requestProvider { get; set; }

        public bool IsValid { get; set; }
        public INavigation Navigation { get; set;}
        
        private ValidatableObject<string> code;
        private ValidatableObject<string> newPassword;
        private ValidatableObject<string> newRepeatPassword;
        
        private string alertErrorMessage;
        private string alertSuccessMessage;
        
        public NewPasswordViewModel(INavigation navigation, ForgotPasswordModel forgotPasswordModel)
        {
            this.Navigation = navigation;
            alertErrorMessage = "Error!";
            alertSuccessMessage = "Success!";
            
            requestProvider = new RequestProvider();
            
            this.forgotPasswordConfirmModel = new ForgotPasswordConfirmModel();
            forgotPasswordConfirmModel.Email = forgotPasswordModel.Email;

            code = new ValidatableObject<string>();
            newPassword = new ValidatableObject<string>();
            newRepeatPassword = new ValidatableObject<string>();
            
            AddValidations();
        }
        
        public ValidatableObject<string> Code
        {
            get
            {
                forgotPasswordConfirmModel.Code = code.Value;
                
                return code;
            }
            set
            {
                if (code.Value != value.Value)
                {
                    code = value;
                    
                    OnPropertyChanged("Code");
                }
            }
        }

        public ValidatableObject<string> NewPassword
        {
            get
            {
                forgotPasswordConfirmModel.Password = newPassword.Value;
                
                return newPassword;
            }
            set
            {
                if (newPassword.Value != value.Value)
                {
                    newPassword = value; 
                    
                    OnPropertyChanged("NewPassword");
                }
            }
        }
        
        public ValidatableObject<string> NewRepeatPassword
        {
            get
            {
                return newRepeatPassword;
            }
            set
            {
                if (newRepeatPassword.Value != value.Value)
                {
                    newRepeatPassword = value;

                    OnPropertyChanged("NewRepeatPassword");
                }
            }
        }

        private async Task ChangePassword()
        {
            IsBusy = true;
            IsValid = Validate();

            if (!IsValid)
            {
                string error = string.Empty;
                
                error += Code.Errors.Count > 0 ? Code.Errors[0] : string.Empty;
                error += NewPassword.Errors.Count > 0 ? NewPassword.Errors[0] : string.Empty;
                error += NewRepeatPassword.Errors.Count > 0 ? NewRepeatPassword.Errors[0] : string.Empty;

                await Application.Current.MainPage.DisplayAlert(alertErrorMessage, Environment.NewLine + error, "Ok");
            }
            else
            {
                try
                {
                    var result = await requestProvider
                        .PostAsync<ForgotPasswordConfirmModel, ResultModel>(GlobalSetting.Instance.ForgotPasswordConfirmationEndpoint, forgotPasswordConfirmModel);

                    if (!result.Result)
                    {
                        await Application.Current.MainPage.DisplayAlert(alertErrorMessage, Environment.NewLine + result.Message, "Ok");
                    }
                    else
                    {
                        await Application.Current.MainPage
                            .DisplayAlert(alertSuccessMessage, Environment.NewLine + "The user password has been successfully changed", "Ok")
                            .ContinueWith(t =>
                            {
                                Navigation.PushAsync(new LoginPage());
                            }, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert(alertErrorMessage, Environment.NewLine + e.Message, "Ok");
                }
            }

            IsBusy = false;
        }
        
        private bool Validate()
        {
            var isValid = Code.Validate() && NewPassword.Validate() && NewRepeatPassword.Validate();
            
            if(isValid && newPassword.Value != newRepeatPassword.Value)
            {
                NewPassword.Errors.Add("Password and re-password are not equals");
                
                isValid = false;
            }

            return isValid;
        }

        private void AddValidations()
        {
            Code.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Code is required" });
            NewPassword.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Password is required" });
            NewRepeatPassword.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Repeat password is required" });
        }
    }
}
