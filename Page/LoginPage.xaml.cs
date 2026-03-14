using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutureBound.Page
{
    // LoginPage 
    // Manages local storage of registered accounts and passwords via MAUI Preferences
    public partial class LoginPage : ContentPage
    {
        // Key for storing list of registered usernames in local preferences
        private const string RegisteredAccountsKey = "RegisteredAccounts";
        // Prefix for password storage (prevents key collision between different user accounts)
        private const string UserPasswordKeyPrefix = "UserPassword_";

        // Initialize login page components and load saved accounts
        // Sets up event listeners for account selection and password input changes
        public LoginPage()
        {
            InitializeComponent();
            LoadRegisteredAccounts();

            // Listen for account selection changes to update button states
            pickerAccount.SelectedIndexChanged += (s, e) =>
            {
                UpdateLoginButtonState();
                UpdateDeleteButtonState(); // Sync delete button state
            };

            // Update login button state when password input changes
            entryPassword.TextChanged += (s, e) => UpdateLoginButtonState();

            // Initialize button states on page load
            UpdateLoginButtonState();
            UpdateDeleteButtonState();
        }

        // Refresh account list on UI thread (called from RegisterPage after new account creation)
        // Automatically selects the newest registered account
        public void RefreshAccountList()
        {
            Dispatcher.Dispatch(() =>
            {
                LoadRegisteredAccounts();
                UpdateLoginButtonState();
                UpdateDeleteButtonState(); // Update delete button after refresh

                // Auto-select the latest registered account if list is not empty
                if (pickerAccount.Items.Count > 0)
                {
                    pickerAccount.SelectedIndex = pickerAccount.Items.Count - 1;
                }
            });
        }

        //Load registered usernames from local preferences into account picker
        //Clears existing items to avoid duplicates
        private void LoadRegisteredAccounts()
        {
            pickerAccount.Items.Clear();
            string savedUsernames = Preferences.Get(RegisteredAccountsKey, "");

            if (!string.IsNullOrEmpty(savedUsernames))
            {
                List<string> usernames = savedUsernames.Split(',').ToList();
                foreach (string username in usernames)
                {
                    if (!string.IsNullOrEmpty(username))
                    {
                        pickerAccount.Items.Add(username);
                    }
                }
            }
        }

        // Update login button state (enabled/disabled) based on input validation
        // Button is enabled only when account is selected and password is entered
        private void UpdateLoginButtonState()
        {
            bool isEnabled = !string.IsNullOrEmpty(pickerAccount.SelectedItem?.ToString())
                            && !string.IsNullOrEmpty(entryPassword.Text);

            btnLogin.IsEnabled = isEnabled;
            btnLogin.BackgroundColor = isEnabled ? Color.FromArgb("#2E86AB") : Colors.Gray;
        }

        // Update delete button state (enabled/disabled)
        // Button is enabled only when an account is selected
        private void UpdateDeleteButtonState()
        {
            bool isEnabled = !string.IsNullOrEmpty(pickerAccount.SelectedItem?.ToString());

            btnDeleteAccount.IsEnabled = isEnabled;
            btnDeleteAccount.BackgroundColor = isEnabled ? Color.FromRgb(255, 69, 0) : Colors.Gray;
        }

        // Handle login button click event
        // Validates entered password against saved password for selected account
        // Navigates to MainPage on successful validation
        // <param name="sender">Button that triggered the event</param>
        // <param name="e">Event arguments</param>
        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            string selectedUsername = pickerAccount.SelectedItem?.ToString();
            string inputPassword = entryPassword.Text;

            // Validate required fields
            if (string.IsNullOrEmpty(selectedUsername) || string.IsNullOrEmpty(inputPassword))
            {
                await DisplayAlert("Notification", "Please select a username and enter password", "OK");
                return;
            }

            // Simulate API call delay for authentication
            await Task.Delay(1000);

            // Retrieve saved password for selected account
            string savedPassword = Preferences.Get($"{UserPasswordKeyPrefix}{selectedUsername}", "");

            // Password validation
            if (inputPassword == savedPassword)
            {
                await DisplayAlert("Success", "Login successful!", "OK");
                await Navigation.PushAsync(new MainPage());
            }
            else
            {
                await DisplayAlert("Failed", "Incorrect password, please try again", "OK");
                entryPassword.Text = ""; // Clear password input on failure
            }
        }

        // Navigate to registration page and pass current LoginPage instance
        // Allows RegisterPage to trigger account list refresh after new registration
        // <param name="sender">Button that triggered the event</param>
        // <param name="e">Event arguments</param>
        private async void BtnRegister_Clicked(object sender, EventArgs e)
        {
            var registerPage = new RegisterPage(this);
            await Navigation.PushAsync(registerPage);
        }

        // Handle account deletion button click event
        // Removes selected account and associated password from local storage
        // Includes confirmation dialog to prevent accidental deletion
        // <param name="sender">Button that triggered the event</param>
        // <param name="e">Event arguments</param>
        private async void BtnDeleteAccount_Clicked(object sender, EventArgs e)
        {
            string selectedUsername = pickerAccount.SelectedItem?.ToString();

            // Validate account selection
            if (string.IsNullOrEmpty(selectedUsername))
            {
                await DisplayAlert("Notification", "Please select an account to delete", "OK");
                return;
            }

            // Confirm deletion action (prevents accidental removal)
            bool confirm = await DisplayAlert("Confirm Deletion", $"Are you sure to delete account {selectedUsername}?\nThis action cannot be undone!", "Confirm", "Cancel");
            if (!confirm) return;

            // 1. Retrieve all registered accounts and remove selected account
            string savedAccounts = Preferences.Get(RegisteredAccountsKey, "");
            List<string> accounts = string.IsNullOrEmpty(savedAccounts)
                ? new List<string>()
                : savedAccounts.Split(',').ToList();
            accounts.Remove(selectedUsername);

            // 2. Save updated account list to local storage
            Preferences.Set(RegisteredAccountsKey, string.Join(',', accounts));

            // 3. Remove password associated with deleted account
            Preferences.Remove($"{UserPasswordKeyPrefix}{selectedUsername}");

            // 4. Reset UI state after deletion
            pickerAccount.SelectedIndex = -1; // Clear account selection
            entryPassword.Text = ""; // Clear password input
            LoadRegisteredAccounts(); // Reload account list
            UpdateLoginButtonState();
            UpdateDeleteButtonState();

            // 5. Notify user of successful deletion
            await DisplayAlert("Success", $"Account {selectedUsername} has been deleted!", "OK");
        }
    }
}
