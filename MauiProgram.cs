using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Plugin.LocalNotification;

namespace FutureBound
{
    /// <summary>
    /// Configures and builds the MAUI application instance
    /// Registers core services, fonts, and third-party plugins
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Creates and configures the MAUI app builder
        /// </summary>
        /// <returns>Configured MauiApp instance</returns>
        /// <remarks>
        /// Core Configurations:
        /// - Registers main App class as root application
        /// - Initializes LocalNotification plugin for push notifications
        /// - Configures custom fonts (OpenSans Regular/Semibold)
        /// - Adds debug logging (only in DEBUG build configuration)
        /// </remarks>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseLocalNotification() // Initialize local notification plugin
                .ConfigureFonts(fonts =>
                {
                    // Register custom fonts with friendly names
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            // Add debug logging only in debug builds
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
