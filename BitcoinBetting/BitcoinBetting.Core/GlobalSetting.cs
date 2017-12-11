using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BitcoinBetting.Core
{
    public sealed class GlobalSetting
    {
        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
                
            }

            internal static readonly GlobalSetting Instance = new GlobalSetting();
        }
        
        public static GlobalSetting Instance => Nested.Instance;

        public const string DefaultEndpoint = "http://bitcoinapp.com:50276";
        private string _baseEndpoint;

        public string RegisterEndpoint { get; set; }

        public string LoginEndpoint { get; set; }
        public string LogoutEndpoint { get; set; }
        
        public string RefreshTokenEndpoint { get; set; }

        public string ExternalLoginEndpoint { get; set; }
        public string ExternalLoginFacebookEndpoint { get; set; }
        public string ExternalLoginGoogleEndpoint { get; set; }
        public string ExternalLoginCallbackEndpoint { get; set; }
        public string ExternalLoginConfirmationEndpoint { get; set; }
        public string ExternalLoginFinalEndpoint { get; set; }
        public string ExternalLoginNextEndpoint { get; set; }
        
        public string UserGetDataEndpoint { get; set; }
        
        public string ForgotPasswordEndpoint { get; set; }
        public string ForgotPasswordConfirmationEndpoint { get; set; }
        
        public string AddressCreateWaletEndpoint { get; set; }
        public string AddressGetWaletEndpoint { get; set; }
        
        public string BettingGetEndpoint { get; set; }
        
        public string BidCreateEndpoint { get; set; }
        public string BidGetEndpoint { get; set; }
        public string BidGetByIdEndpoint { get; set; }
        
        private GlobalSetting()
        {
            BaseEndpoint = DefaultEndpoint;
        }

        public string BaseEndpoint
        {
            get => _baseEndpoint;
            
            private set
            {
                _baseEndpoint = value;
                
                UpdateEndpoint(_baseEndpoint);
            }
        }

        private void UpdateEndpoint(string baseEndpoint)
        {
            RegisterEndpoint = string.Format("{0}/api/Account/Register", baseEndpoint);
            LoginEndpoint = string.Format("{0}/api/Account/Login", baseEndpoint);
            LogoutEndpoint = string.Format("{0}/api/Account/Logout", baseEndpoint);
            RefreshTokenEndpoint = string.Format("{0}/api/Account/RefreshToken", baseEndpoint);
            
            ExternalLoginEndpoint = string.Format("{0}/api/Account/ExternalLogin", baseEndpoint);
            ExternalLoginCallbackEndpoint = string.Format("{0}/api/Account/ExternalLoginCallback", baseEndpoint);
            ExternalLoginConfirmationEndpoint = string.Format("{0}/api/Account/ExternalLoginConfirmation", baseEndpoint);
            ExternalLoginNextEndpoint = string.Format("{0}/api/Account/next", baseEndpoint);
            ExternalLoginFinalEndpoint = string.Format("{0}/api/Account/final", baseEndpoint);

            ExternalLoginFacebookEndpoint = string.Format("{0}?provider={1}&deviceId={2}", ExternalLoginEndpoint, "Facebook", (string) Application.Current.Properties["device_id"]);
            ExternalLoginGoogleEndpoint = string.Format("{0}?provider={1}&deviceId={2}", ExternalLoginEndpoint, "Google", (string) Application.Current.Properties["device_id"]);
            
            ForgotPasswordEndpoint = string.Format("{0}/api/Account/ForgotPassword", baseEndpoint);
            ForgotPasswordConfirmationEndpoint = string.Format("{0}/api/Account/ForgotPasswordConfirmation", baseEndpoint);
            
            UserGetDataEndpoint = string.Format("{0}/api/Users/GetUserData", baseEndpoint);
            
            AddressCreateWaletEndpoint = string.Format("{0}/api/Wallet/Create", baseEndpoint);
            AddressGetWaletEndpoint = string.Format("{0}/api/Wallet/Get", baseEndpoint);
            
            BettingGetEndpoint = string.Format("{0}/api/Betting/Get", baseEndpoint);
            
            BidCreateEndpoint = string.Format("{0}/api/Bid/Create", baseEndpoint);
            BidGetEndpoint = string.Format("{0}/api/Bid/Get", baseEndpoint);
            BidGetByIdEndpoint = string.Format("{0}/api/Bid/GetById", baseEndpoint);
        }
    }
}
