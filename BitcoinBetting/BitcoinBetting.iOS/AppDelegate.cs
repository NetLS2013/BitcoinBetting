using System;
using System.Collections.Generic;
using System.Linq;

using BitcoinBetting.Core;
using BitcoinBetting.Core.Views.Account;
using Foundation;
using UIKit;

namespace BitcoinBetting.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        private App application;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            application = new App();
            LoadApplication(application);

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            var uri = new NSUrlComponents(url, true);
            var token = uri.PercentEncodedQueryItems.FirstOrDefault(x => x.Name == "token")?.Value;

            if (!string.IsNullOrWhiteSpace(token))
            {
                Core.GlobalSetting.Instance.AuthToken = token;
                // TODO Open next page

                LoadApplication(application);
            }
            else
            {
                
                var model = new Core.Models.User.ExternalLoginConfirmModel();
                model.Email = uri.PercentEncodedQueryItems.FirstOrDefault(x => x.Name == "email").Value;
                model.FirstName = uri.PercentEncodedQueryItems.FirstOrDefault(x => x.Name == "gname").Value;
                model.LastName = uri.PercentEncodedQueryItems.FirstOrDefault(x => x.Name == "sname").Value;
                model.Cookie = uri.PercentEncodedQueryItems.FirstOrDefault(x => x.Name == "externalToken").Value;
                model.Provider = uri.PercentEncodedQueryItems.FirstOrDefault(x => x.Name == "provider").Value;

                LoadApplication(application);

                Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new ExtrenalLoginConfirmPage(model));
            }

            return true;
        }
    }

}
