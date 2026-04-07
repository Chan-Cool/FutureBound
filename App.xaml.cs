using Microsoft.Maui.Controls;
using FutureBound.Pages;
using FutureBound.Data;
using CommunityToolkit.Maui;

namespace FutureBound
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Initialize the MAUI Community Toolkit
            MauiProgram.CreateMauiApp();
            MainPage = new NavigationPage(new FutureBound.Page.SplashPage());

            // Request notification permissions (when the APP starts)
            _ = NotificationHelper.RequestNotificationPermissionAsync();
        }
    }
}
