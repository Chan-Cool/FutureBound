using Microsoft.Maui.Controls;
using FutureBound.Pages;
using FutureBound.Data;
using Plugin.LocalNotification;

namespace FutureBound
{
    /// <summary>
    /// Main application class - manages app lifecycle and navigation
    /// Handles notification permission requests and notification tap navigation
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes app components, sets root page, and configures notifications
        /// </summary>
        public App()
        {
            InitializeComponent();
            // Set root page to LoginPage wrapped in NavigationPage
            MainPage = new NavigationPage(new FutureBound.Page.LoginPage());

            // Request notification permission on app startup (fire and forget)
            _ = NotificationHelper.RequestNotificationPermissionAsync();

            // Register handler for notification tap events (navigation logic)
            LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationActionTapped;
        }

        /// <summary>
        /// Handles notification tap events - navigates to specified page
        /// </summary>
        /// <param name="e">Notification action event arguments (contains navigation data)</param>
        /// <remarks>
        /// Execution Context:
        /// - Runs on main thread to ensure UI thread safety
        /// - Only processes navigation if MainPage is a NavigationPage
        /// 
        /// Navigation Logic:
        /// - ReturnToHome: Navigates to HomePage
        /// - ReturnToBill: Navigates to BillPage (BillDetailPage requires Bill object)
        /// </remarks>
        private void OnNotificationActionTapped(Plugin.LocalNotification.EventArgs.NotificationActionEventArgs e)
        {
            var returningData = e.Request.ReturningData;

            // Execute navigation on main thread (required for UI operations)
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (MainPage is NavigationPage nav)
                {
                    if (returningData == NotificationHelper.ReturnToHome)
                    {
                        // Navigate to HomePage when notification is tapped
                        await nav.Navigation.PushAsync(new FutureBound.Pages.HomePage());
                    }
                    else if (returningData == NotificationHelper.ReturnToBill)
                    {
                        // Navigate to BillPage (BillDetailPage needs Bill object - cannot navigate directly)
                        await nav.Navigation.PushAsync(new FutureBound.Page.BillPage());
                    }
                }
            });
        }
    }
}
