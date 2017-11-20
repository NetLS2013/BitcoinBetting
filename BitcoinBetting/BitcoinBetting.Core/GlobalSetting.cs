using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinBetting.Core
{
    public class GlobalSetting
    {
        public const string DefaultEndpoint = "http://bitcoinapp.com:50276";
        private string baseEndpoint;

        public string AuthToken { get; set; }

        public string RegisterEndpoint { get; set; }

        public string LoginEndpoint { get; set; }
        public string LogoutEndpoint { get; set; }

        public string ExternalLoginEndpoint { get; set; }
        public string ExternalLoginFacebookEndpoint { get; set; }
        public string ExternalLoginGoogleEndpoint { get; set; }
        public string ExternalLoginCallbackEndpoint { get; set; }
        public string ExternalLoginConfirmationEndpoint { get; set; }
        public string ExternalLoginFinalEndpoint { get; set; }
        public string ExternalLoginNextEndpoint { get; set; }
        
        public string ForgotPasswordEndpoint { get; set; }
        public string ForgotPasswordConfirmationEndpoint { get; set; }
        
        public string AddressCreateWaletEndpoint { get; set; }
        public string AddressGetWaletEndpoint { get; set; }

        private static object myLock = new object();
        private static volatile GlobalSetting instance;

        private GlobalSetting() { }

        public static GlobalSetting Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (myLock)
                    {
                        if (instance == null)
                        {
                            instance = new GlobalSetting()
                            {
                                BaseEndpoint = DefaultEndpoint
                            };
                        }
                    }
                }

                return instance;
            }
        }

        public string BaseEndpoint
        {
            get { return baseEndpoint; }
            set
            {
                baseEndpoint = value;
                
                UpdateEndpoint(baseEndpoint);
            }
        }

        private void UpdateEndpoint(string baseEndpoint)
        {
            RegisterEndpoint = string.Format("{0}/api/Account/Register", baseEndpoint);
            LoginEndpoint = string.Format("{0}/api/Account/Login", baseEndpoint);
            LogoutEndpoint = string.Format("{0}/api/Account/Logout", baseEndpoint);
            ExternalLoginEndpoint = string.Format("{0}/api/Account/ExternalLogin", baseEndpoint);
            ExternalLoginCallbackEndpoint = string.Format("{0}/api/Account/ExternalLoginCallback", baseEndpoint);
            ExternalLoginConfirmationEndpoint = string.Format("{0}/api/Account/ExternalLoginConfirmation", baseEndpoint);
            ExternalLoginNextEndpoint = string.Format("{0}/api/Account/next", baseEndpoint);
            ExternalLoginFinalEndpoint = string.Format("{0}/api/Account/final", baseEndpoint);

            ExternalLoginFacebookEndpoint = string.Format("{0}?provider={1}", ExternalLoginEndpoint, "Facebook");
            ExternalLoginGoogleEndpoint = string.Format("{0}?provider={1}", ExternalLoginEndpoint, "Google");
            
            ForgotPasswordEndpoint = string.Format("{0}/api/Account/ForgotPassword", baseEndpoint);
            ForgotPasswordConfirmationEndpoint = string.Format("{0}/api/Account/ForgotPasswordConfirmation", baseEndpoint);
            
            AddressCreateWaletEndpoint = string.Format("{0}/api/Wallet/Create", baseEndpoint);
            AddressGetWaletEndpoint = string.Format("{0}/api/Wallet/Get", baseEndpoint);
        }

    }
}
