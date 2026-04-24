using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutureBound.Page
{
    // RegisterPage - Handles new user account creation
    // Saves username and password to local storage, prevents duplicate registrations
    // Communicates with LoginPage to refresh account list after successful registration
    public partial class RegisterPage : ContentPage
    {
        private const string RegisteredAccountsKey   = "RegisteredAccounts";
        private const string UserPasswordKeyPrefix    = "UserPassword_";
        private const string SecurityQuestionPrefix   = "SecurityQuestion_";
        private const string SecurityAnswerPrefix     = "SecurityAnswer_";

        // Reference to parent LoginPage for account list refresh
        private LoginPage _loginPage;

        // Initialize registration page and store reference to LoginPage
        // <param name="loginPage">Instance of LoginPage to trigger refresh</param>
        public RegisterPage(LoginPage loginPage)
        {
            InitializeComponent();
            _loginPage = loginPage;
        }

        // Handle register button click event
        // Validates input, checks for duplicate usernames, saves account credentials to local storage
        // Refreshes LoginPage account list after successful registration
        // <param name="sender">Button that triggered the event</param>
        // <param name="e">Event arguments</param>
        private async void BtnRegister_Clicked(object sender, EventArgs e)
        {
            // Get and trim user input to remove extra whitespace
            string username         = entryUsername.Text?.Trim();
            string password         = entryPassword.Text?.Trim();
            string securityQuestion = entrySecurityQuestion.Text?.Trim();
            string securityAnswer   = entrySecurityAnswer.Text?.Trim();

            // Validate required fields
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Notification", "Please fill in both username and password", "OK");
                return;
            }

            if (string.IsNullOrEmpty(securityQuestion) || string.IsNullOrEmpty(securityAnswer))
            {
                await DisplayAlert("Notification", "Please fill in a security question and answer", "OK");
                return;
            }

            // Retrieve existing registered accounts from local storage
            string savedAccounts = Preferences.Get(RegisteredAccountsKey, "");
            List<string> accounts = string.IsNullOrEmpty(savedAccounts)
                ? new List<string>()
                : savedAccounts.Split(',').ToList();

            // Check for duplicate username (prevent duplicate registrations)
            if (accounts.Contains(username))
            {
                await DisplayAlert("Notification", "This username is already registered", "OK");
                return;
            }

            // 1. Add new username to account list and save to local storage
            accounts.Add(username);
            Preferences.Set(RegisteredAccountsKey, string.Join(',', accounts));

            // 2. Save password
            Preferences.Set($"{UserPasswordKeyPrefix}{username}", password);

            // 3. Save security question and answer (answer stored in lower case for case-insensitive match)
            Preferences.Set($"{SecurityQuestionPrefix}{username}", securityQuestion);
            Preferences.Set($"{SecurityAnswerPrefix}{username}", securityAnswer.ToLower());

            // Notify user of successful registration
            await DisplayAlert("Success", $"Username {username} registered successfully!", "OK");

            // Navigate back to LoginPage
            await Navigation.PopAsync();

            // Small delay to ensure data persistence before refresh
            await Task.Delay(100);

            // Trigger account list refresh on LoginPage if reference exists
            if (_loginPage != null)
            {
                _loginPage.RefreshAccountList();
            }
        }

        // Handle login button click event (navigate back to LoginPage)
        // Refreshes account list on LoginPage to ensure latest data is displayed
        // <param name="sender">Button that triggered the event</param>
        // <param name="e">Event arguments</param>
        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            // Navigate back to LoginPage without registration
            await Navigation.PopAsync();

            // Small delay to ensure smooth UI transition before refresh
            await Task.Delay(100);

            // Refresh account list on LoginPage if reference exists
            if (_loginPage != null)
            {
                _loginPage.RefreshAccountList();
            }
        }
    }
}