using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinBetting.Core
{
    public class GlobalSetting
    {
        public const string DefaultEndpoint = "http://5454958b.ngrok.io";
        private string baseEndpoint;

        public string AuthToken { get; set; }

        public string RegisterEndpoint { get; set; }

        public string LoginEndpoint { get; set; }

        public string LogoutEndpoint { get; set; }

        private static object myLock = new object();
        private static volatile GlobalSetting instance = new GlobalSetting();

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
                            instance = new GlobalSetting() {
                                BaseEndpoint = DefaultEndpoint
                            };
                            
                        }
                    }
                }

                instance.UpdateEndpoint(DefaultEndpoint);
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
        }

    }
}
