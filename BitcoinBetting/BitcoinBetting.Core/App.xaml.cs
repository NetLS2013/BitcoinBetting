﻿using System;
using System.Linq;
using BitcoinBetting.Core.Views.Menu;
using Xamarin.Forms;

namespace BitcoinBetting.Core
{
    public partial class App : Application
    {
        public App(string token, string refreshToken)
        {
            InitializeComponent();
            
            Properties["token"] = token;
            Properties["refresh_token"] = refreshToken;
            
            MainPage = new MasterPage();
        }

        public App()
        {
            InitializeComponent();
            
            if (Properties.ContainsKey("token"))
            {
                MainPage = new MasterPage();
            }
            else
            {
                MainPage = new NavigationPage(new StartupPage())
                {
                    BarBackgroundColor = Color.Transparent,
                    BarTextColor = Color.Black
                };
            }
        }

        protected override void OnStart()
        {
            if (!Properties.ContainsKey("device_id"))
            {
                Properties.Add("device_id", Guid.NewGuid().ToString().Replace("-", ""));
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
