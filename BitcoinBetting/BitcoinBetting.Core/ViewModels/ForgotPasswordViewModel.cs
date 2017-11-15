using BitcoinBetting.Core.Interfaces;
using BitcoinBetting.Core.Models.User;
using BitcoinBetting.Core.Services;
using BitcoinBetting.Core.Services.Validations;
using BitcoinBetting.Core.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using BitcoinBetting.Core.Views;
using Xamarin.Forms;

namespace BitcoinBetting.Core.ViewModels
{
    public class ForgotPasswordViewModel : BaseViewModel
    {
        private ForgotPasswordModel forgotPasswordModel;

        public ICommand ForgotPasswordCommand => new Command(async () => await ForgotPassword());
        
        public IRequestProvider requestProvider { get; set; }

        public bool IsValid { get; set; }
        public INavigation Navigation { get; set;}
        public Page CurrentPage { get; set;}
        
        private ValidatableObject<string> email;

        private string alertErrorMessage;
        
        public ForgotPasswordViewModel(INavigation navigation, Page currentPage)
        {
            this.Navigation = navigation;
            this.CurrentPage = currentPage;

            alertErrorMessage = "Error";
            
            requestProvider = new RequestProvider();
            forgotPasswordModel = new ForgotPasswordModel();

            email = new ValidatableObject<string>();
                
            AddValidations();
        }

        public ValidatableObject<string> Email
        {
            get
            {
                forgotPasswordModel.Email = email.Value;
                
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
        
        private async Task ForgotPassword()
        {
            IsBusy = true;
            IsValid = Validate();

            if (!IsValid)
            {
                string error = string.Empty;
                
                error += Email.Errors.Count > 0 ? Environment.NewLine + Email.Errors[0] : string.Empty;

                await Application.Current.MainPage.DisplayAlert(alertErrorMessage, error, "Ok");
            }
            else
            {
                try
                {
                    var result = await requestProvider.PostAsync<ForgotPasswordModel, Result>(GlobalSetting.Instance.ForgotPasswordEndpoint, forgotPasswordModel);

                    if (!result.result)
                    {
                        await Application.Current.MainPage.DisplayAlert(alertErrorMessage, Environment.NewLine + result.Message, "Ok");
                    }
                    else
                    {
                        await Navigation.PushAsync(new NewPasswordPage(forgotPasswordModel));
                        Navigation.RemovePage(CurrentPage);
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
            return Email.Validate();
        }

        private void AddValidations()
        {
            Email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A email is required" });
        }
    }
}
