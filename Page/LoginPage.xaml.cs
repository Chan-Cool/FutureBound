using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FutureBound.Data;

namespace FutureBound.Page
{
    /// <summary>
    /// LoginPage - User authentication and account management page
    /// Core functions:
    /// - Load/save registered accounts via MAUI Preferences
    /// - Validate user credentials (username/password)
    /// - Navigate to registration page
    /// - Delete existing accounts with confirmation
    /// - Manage UI state for buttons based on input validation
    /// </summary>
    public partial class LoginPage : ContentPage
    {
        private const string RegisteredAccountsKey = "RegisteredAccounts";
        private const string UserPasswordKeyPrefix  = "UserPassword_";
        private const string SecurityQuestionPrefix = "SecurityQuestion_";
        private const string SecurityAnswerPrefix   = "SecurityAnswer_";

        // Secondary-password attempt tracking (per session, resets on app restart)
        private int _deleteAttempts = 0;
        private const int MaxDeleteAttempts = 3;
        private DateTime _lockUntil = DateTime.MinValue;

        /// <summary>
        /// Initialize login page components and load saved accounts
        /// Sets up event listeners for account selection and password input changes
        /// </summary>
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

        /// <summary>
        /// Refresh account list on UI thread (called from RegisterPage after new account creation)
        /// Automatically selects the newest registered account
        /// </summary>
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

        /// <summary>
        /// Load registered usernames from local preferences into account picker
        /// Clears existing items to avoid duplicates
        /// </summary>
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

        /// <summary>
        /// Update login button state (enabled/disabled) based on input validation
        /// Button is enabled only when account is selected and password is entered
        /// </summary>
        private void UpdateLoginButtonState()
        {
            bool isEnabled = !string.IsNullOrEmpty(pickerAccount.SelectedItem?.ToString())
                            && !string.IsNullOrEmpty(entryPassword.Text);

            btnLogin.IsEnabled = isEnabled;
            btnLogin.BackgroundColor = isEnabled ? Color.FromArgb("#2E86AB") : Colors.Gray;
        }

        /// <summary>
        /// Update delete button state (enabled/disabled)
        /// Button is enabled only when an account is selected
        /// </summary>
        private void UpdateDeleteButtonState()
        {
            bool isEnabled = !string.IsNullOrEmpty(pickerAccount.SelectedItem?.ToString());

            btnDeleteAccount.IsEnabled = isEnabled;
            btnDeleteAccount.BackgroundColor = isEnabled ? Color.FromRgb(255, 69, 0) : Colors.Gray;
        }

        /// <summary>
        /// Handle login button click event
        /// Validates entered password against saved password for selected account
        /// Navigates to MainPage on successful validation
        /// </summary>
        /// <param name="sender">Button that triggered the event</param>
        /// <param name="e">Event arguments</param>
        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            string selectedUsername = pickerAccount.SelectedItem?.ToString();
            string inputPassword = entryPassword.Text;

            if (string.IsNullOrEmpty(selectedUsername) || string.IsNullOrEmpty(inputPassword))
            {
                await DisplayAlert("Notification", "Please select a username and enter password", "OK");
                return;
            }

            await Task.Delay(1000);

            string savedPassword = Preferences.Get($"{UserPasswordKeyPrefix}{selectedUsername}", "");

            if (inputPassword == savedPassword)
            {
                // ✅ Add this line! Set current username after successful login
                AccountContext.CurrentUsername = selectedUsername;

                await DisplayAlert("Success", "Login successful!", "OK");
                await Navigation.PushAsync(new FutureBound.Pages.HomePage());
            }
            else
            {
                await DisplayAlert("Failed", "Incorrect password, please try again", "OK");
                entryPassword.Text = "";
            }
        }

        /// <summary>
        /// Navigate to registration page and pass current LoginPage instance
        /// Allows RegisterPage to trigger account list refresh after new registration
        /// </summary>
        /// <param name="sender">Button that triggered the event</param>
        /// <param name="e">Event arguments</param>
        private async void BtnRegister_Clicked(object sender, EventArgs e)
        {
            var registerPage = new RegisterPage(this);
            await Navigation.PushAsync(registerPage);
        }

        /// <summary>
        /// Handle forgot password tap
        /// Flow: check lock → load security question → verify answer → set new password
        /// Shares the same lockout counter as the delete function
        /// </summary>
        private async void BtnForgotPassword_Clicked(object sender, EventArgs e)
        {
            // ── Step 1: Must have an account selected ─────────────────────
            string selectedUsername = pickerAccount.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedUsername))
            {
                await DisplayAlert("Tip", "Please select an account first.", "OK");
                return;
            }

            // ── Step 2: Check shared lockout ──────────────────────────────
            if (DateTime.Now < _lockUntil)
            {
                int secondsLeft = (int)(_lockUntil - DateTime.Now).TotalSeconds;
                await DisplayAlert("Locked",
                    $"Too many incorrect attempts.\nPlease wait {secondsLeft} second(s) before trying again.",
                    "OK");
                return;
            }

            // ── Step 3: Load and display the security question ────────────
            string question = Preferences.Get($"{SecurityQuestionPrefix}{selectedUsername}", "");
            if (string.IsNullOrEmpty(question))
            {
                await DisplayAlert("Unavailable",
                    "No security question was set for this account.\nPlease contact support or re-register.",
                    "OK");
                return;
            }

            string inputAnswer = await DisplayPromptAsync(
                "Security Question",
                question,
                placeholder: "Your answer",
                maxLength: 100,
                keyboard: Keyboard.Default);

            if (inputAnswer == null) return;

            // ── Step 4: Verify answer (case-insensitive) ──────────────────
            string savedAnswer = Preferences.Get($"{SecurityAnswerPrefix}{selectedUsername}", "");

            if (inputAnswer.Trim().ToLower() != savedAnswer)
            {
                _deleteAttempts++;
                int remaining = MaxDeleteAttempts - _deleteAttempts;

                if (_deleteAttempts >= MaxDeleteAttempts)
                {
                    _lockUntil = DateTime.Now.AddSeconds(30);
                    _deleteAttempts = 0;
                    await DisplayAlert("Locked",
                        "Too many incorrect answers. You have been locked out for 30 seconds.",
                        "OK");
                }
                else
                {
                    await DisplayAlert("Incorrect Answer",
                        $"The answer you entered is incorrect.\nYou have {remaining} attempt(s) remaining.",
                        "OK");
                }
                return;
            }

            // Answer correct — reset counter
            _deleteAttempts = 0;

            // ── Step 5: Prompt for new password ───────────────────────────
            string newPassword = await DisplayPromptAsync(
                "Reset Password",
                "Enter your new password:",
                placeholder: "New password",
                maxLength: 50,
                keyboard: Keyboard.Default);

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                await DisplayAlert("Cancelled", "Password reset was cancelled.", "OK");
                return;
            }

            // ── Step 6: Save new password ─────────────────────────────────
            Preferences.Set($"{UserPasswordKeyPrefix}{selectedUsername}", newPassword.Trim());

            entryPassword.Text = "";
            await DisplayAlert("Success",
                "Your password has been reset successfully.\nPlease log in with your new password.",
                "OK");
        }

        /// <summary>
        /// Handle account deletion button click event
        /// Flow: check lock → prompt secondary password → verify → confirm dialog → delete
        /// Locks deletion for 30 seconds after 3 consecutive wrong passwords
        /// </summary>
        private async void BtnDeleteAccount_Clicked(object sender, EventArgs e)
        {
            string selectedUsername = pickerAccount.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedUsername))
            {
                await DisplayAlert("Notification", "Please select an account to delete", "OK");
                return;
            }

            // ── Step 1: Check if deletion is currently locked ──────────────
            if (DateTime.Now < _lockUntil)
            {
                int secondsLeft = (int)(_lockUntil - DateTime.Now).TotalSeconds;
                await DisplayAlert("Locked",
                    $"Too many incorrect attempts.\nPlease wait {secondsLeft} second(s) before trying again.",
                    "OK");
                return;
            }

            // ── Step 2: Prompt for secondary password ──────────────────────
            string inputPassword = await DisplayPromptAsync(
                "Verify Identity",
                $"Enter the password for \"{selectedUsername}\" to proceed:",
                placeholder: "Password",
                maxLength: 50,
                keyboard: Keyboard.Default);

            if (inputPassword == null) return; // user cancelled

            // ── Step 3: Validate password ──────────────────────────────────
            string savedPassword = Preferences.Get($"{UserPasswordKeyPrefix}{selectedUsername}", "");

            if (inputPassword != savedPassword)
            {
                _deleteAttempts++;
                int remaining = MaxDeleteAttempts - _deleteAttempts;

                if (_deleteAttempts >= MaxDeleteAttempts)
                {
                    _lockUntil = DateTime.Now.AddSeconds(30);
                    _deleteAttempts = 0;
                    await DisplayAlert("Locked",
                        "Incorrect password. You have been locked out for 30 seconds.",
                        "OK");
                }
                else
                {
                    await DisplayAlert("Incorrect Password",
                        $"The password you entered is incorrect.\nYou have {remaining} attempt(s) remaining.",
                        "OK");
                }
                return;
            }

            // Password correct — reset attempt counter
            _deleteAttempts = 0;

            // ── Step 4: Final confirmation dialog ─────────────────────────
            bool confirm = await DisplayAlert(
                "Confirm Deletion",
                $"Are you sure you want to permanently delete account \"{selectedUsername}\"?\nThis action cannot be undone!",
                "Delete", "Cancel");

            if (!confirm) return;

            // ── Step 5: Delete account data and update UI ──────────────────
            string savedAccounts = Preferences.Get(RegisteredAccountsKey, "");
            List<string> accounts = string.IsNullOrEmpty(savedAccounts)
                ? new List<string>()
                : savedAccounts.Split(',').ToList();
            accounts.Remove(selectedUsername);

            Preferences.Set(RegisteredAccountsKey, string.Join(',', accounts));
            Preferences.Remove($"{UserPasswordKeyPrefix}{selectedUsername}");
            AccountDataManager.DeleteAccountData(selectedUsername);

            pickerAccount.SelectedIndex = -1;
            entryPassword.Text = "";
            LoadRegisteredAccounts();
            UpdateLoginButtonState();
            UpdateDeleteButtonState();

            await DisplayAlert("Deleted", $"Account \"{selectedUsername}\" has been deleted.", "OK");
        }
    }
}