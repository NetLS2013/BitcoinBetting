using System;
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
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Forms.Init(this, bundle);

            var intent = Intent;
            
            if (Intent.ActionView.Equals(intent.Action))
            {
                Android.Net.Uri uri = intent.Data;
                String token = uri.GetQueryParameter("token");
                
                if (!string.IsNullOrWhiteSpace(token))
                {
                    GlobalSetting.Instance.AuthToken = token;
                    Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new MasterPage());

                    LoadApplication(new App());
                }
                else
                {
                    var model = new Core.Models.User.ExternalLoginConfirmModel();
                    model.Email = uri.GetQueryParameter("email");
                    model.FirstName = uri.GetQueryParameter("gname");
                    model.LastName = uri.GetQueryParameter("sname");
                    model.Cookie = uri.GetQueryParameter("externalToken");
                    model.Provider = uri.GetQueryParameter("provider");

                    LoadApplication(new App());

                    Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new ExtrenalLoginConfirmPage(model));
                }
            }
            else
            {
                LoadApplication(new App());
            }
        }
    }
}

