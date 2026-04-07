using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace FutureBound.Data
{
    /// <summary>
    /// Helper class for managing local in-app notifications in .NET MAUI application
    /// Implements course-required MAUI native APIs for notification functionality
    /// Follows best practices for static utility class design
    /// </summary>
    public static class NotificationHelper
    {
        /// <summary>
        /// Requests user permission for post notifications (required for Android 13+)
        /// Called on app launch to ensure notification functionality is available
        /// </summary>
        /// <returns>True if permission granted, false otherwise</returns>
        public static async Task<bool> RequestNotificationPermissionAsync()
        {
            var status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            return status == PermissionStatus.Granted;
        }

        public static void SendImmediateNotification(string title, string message)
        {
            var page = Application.Current?.MainPage;
            if (page == null) return;
            _ = page.DisplayAlert(title, message, "OK");
        }

        /// <summary>
        /// Schedules delayed notification for bill reminder functionality
        /// Allows users to set custom delay for future reminders
        /// </summary>
        /// <param name="page">Current page instance</param>
        /// <param name="title">Reminder notification title</param>
        /// <param name="message">Reminder message with bill details</param>
        /// <param name="delayMinutes">Delay time in minutes before notification triggers</param>
        public static async void ScheduleBillReminder(string title, string message, int delayMinutes)
        {
            await Task.Delay(TimeSpan.FromMinutes(delayMinutes));
            var page = Application.Current?.MainPage;
            if (page == null) return;
            _ = page.DisplayAlert(title, message, "OK");
        }
    }
}