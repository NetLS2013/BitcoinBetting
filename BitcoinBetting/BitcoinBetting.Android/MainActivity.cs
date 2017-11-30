using System;
using System.Linq;
using BitcoinBetting.Core;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using BitcoinBetting.Core.Views.Account;
using BitcoinBetting.Core.Views.Menu;
using Xamarin.Forms;

namespace BitcoinBetting.Droid
{
    [IntentFilter(
       new[] { Intent.ActionView },
       AutoVerify = true,
       Categories = new[]
       {
            Intent.CategoryDefault,
            Intent.CategoryBrowsable
       },
       DataScheme = "bitcoinbetting",
       DataHost = "bitcoinapp.com")]
    [Activity(Label = "BitcoinBetting", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private Page startupPage;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Forms.Init(this, bundle);

            startupPage = new NavigationPage(new StartupPage())
            {
                BarBackgroundColor = Color.Transparent,
                BarTextColor = Color.Black
            };
            
            var intent = Intent;
            
            if (Intent.ActionView.Equals(intent.Action))
            {
                var uri = intent.Data;
                string token = uri.GetQueryParameter("token");
                string refreshToken = uri.GetQueryParameter("refresh_token");
                
                if (!string.IsNullOrWhiteSpace(token))
                {
                    Xamarin.Forms.Application.Current.Properties["token"] = token;
                    Xamarin.Forms.Application.Current.Properties["refresh_token"] = refreshToken;
                    
                    LoadApplication(new App(new MasterPage()));
                }
                else
                {
                    var model = new Core.Models.User.ExternalLoginConfirmModel();
                    model.Email = uri.GetQueryParameter("email");
                    model.FirstName = uri.GetQueryParameter("gname");
                    model.LastName = uri.GetQueryParameter("sname");
                    model.Cookie = uri.GetQueryParameter("externalToken");
                    model.Provider = uri.GetQueryParameter("provider");

                    LoadApplication(new App(startupPage));
                    
                    Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new ExtrenalLoginConfirmPage(model));
                }
            }
            else
            {
                LoadApplication(new App(startupPage));
            }
        }
    }
}

