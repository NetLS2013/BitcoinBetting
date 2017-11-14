using System;
using BitcoinBetting.Core;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;

namespace BitcoinBetting.Droid
{
    [IntentFilter(new[] { Android.Content.Intent.ActionView },
                       AutoVerify = true,
                       Categories = new[]
                       {
                            Android.Content.Intent.CategoryDefault,
                            Android.Content.Intent.CategoryBrowsable
                       },
                       DataScheme = "bitcoinbetting",
                       DataHost = "bitcoinapp.com")]
    [Activity(Label = "BitcoinBetting", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            var intent = this.Intent;
            if (Intent.ActionView.Equals(intent.Action))
            {
                Android.Net.Uri uri = intent.Data;
                String token = uri.GetQueryParameter("token");
                if (!string.IsNullOrWhiteSpace(token))
                {
                    Core.GlobalSetting.Instance.AuthToken = token;
                    // TODO Open next page

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

                    Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new Core.Views.ExtrenalLoginConfirmPage(model));
                }
            }
            else
            {
               
                LoadApplication(new App());
            }

            
        }
    }
}

