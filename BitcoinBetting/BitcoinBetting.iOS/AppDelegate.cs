using System;
using System.Collections.Generic;
using System.Linq;

using BitcoinBetting.Core;
using BitcoinBetting.Core.Views.Account;
using BitcoinBetting.Core.Views.Menu;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace BitcoinBetting.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.Init();
            LoadApplication(new App(new NavigationPage(new StartupPage())));

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            var uri = new NSUrlComponents(url, true);
            var token = uri.PercentEncodedQueryItems.FirstOrDefault(x => x.Name == "token")?.Value;
            var refreshToken = uri.PercentEncodedQueryItems.FirstOrDefault(x => x.Name == "refresh_token")?.Value;

            if (!string.IsNullOrWhiteSpace(token))
            {
                Xamarin.Forms.Application.Current.Properties["token"] = token;
                Xamarin.Forms.Application.Current.Properties["refresh_token"] = refreshToken;
                
                Xamarin.Forms.Application.Current.MainPage = new MasterPage();
            }
            else
            {
                var model = new Core.Models.User.ExternalLoginConfirmModel();
                
                model.Email = uri.PercentEncodedQueryItems.FirstOrDefault(x => x.Name == "email").Value;
                model.FirstName = uri.PercentEncodedQueryItems.FirstOrDefault(x => x.Name == "gname").Value;
                model.LastName = uri.PercentEncodedQueryItems.FirstOrDefault(x => x.Name == "sname").Value;
                model.Cookie = uri.PercentEncodedQueryItems.FirstOrDefault(x => x.Name == "externalToken").Value;
                model.Provider = uri.PercentEncodedQueryItems.FirstOrDefault(x => x.Name == "provider").Value;

                Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new ExtrenalLoginConfirmPage(model));
            }

            return true;
        }
    }

}
