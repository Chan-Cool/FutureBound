using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace FutureBound.Page
{
    // SplashPage - App launch screen with simulated loading delay
    // Displays brand identity during startup, then navigates to LoginPage
    public partial class SplashPage : ContentPage
    {
        // Initialize splash page and start simulated startup sequence
        public SplashPage()
        {
            InitializeComponent();
            // Fire and forget async startup sequence (no need to await in constructor)
            _ = SimulateStartupAsync();
        }

        // Simulate app initialization delay (mimics loading resources/config)
        // Navigates to LoginPage after delay completes
        // <returns>Async task for delay and navigation</returns>
        private async Task SimulateStartupAsync()
        {
            // Simulate 2-second loading process (e.g., API calls, asset loading)
            await Task.Delay(2000);

            // Navigate to LoginPage after startup simulation completes
            await Navigation.PushAsync(new LoginPage());
        }
    }
}