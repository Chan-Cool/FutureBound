using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using FutureBound.Data;
using FutureBound.Pages;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutureBound.Page;

/// <summary>
/// MorePage serves as the central settings hub for the FutureBound personal finance application.
/// Core responsibilities include:
/// - Profile customization (avatar selection and persistence)
/// - Application configuration (decimal precision, date format)
/// - Data lifecycle management (export, local cache clearing)
/// - Account operations (switching between local accounts, secure logout)
/// - Supplementary features (about information, bottom navigation)
/// </summary>
public partial class MorePage : ContentPage
{
    //  Preference Key Constants 
    /// <summary>Prefix for avatar preference keys (scoped to individual users)</summary>
    private const string AvatarKeyPrefix = "Avatar_";
    /// <summary>Prefix for decimal precision preference keys (user-scoped)</summary>
    private const string DecimalKeyPrefix = "DecimalPlaces_";
    /// <summary>Prefix for date format preference keys (user-scoped)</summary>
    private const string DateFormatKeyPrefix = "DateFormat_";
    /// <summary>Preference key for storing registered account list (global scope)</summary>
    private const string RegisteredAccountsKey = "RegisteredAccounts";

    /// <summary>Tracks the currently active popup to manage overlay interactions</summary>
    private Border? _activePopup;

    /// <summary>
    /// Initializes the MorePage component, loads XAML-defined UI elements
    /// </summary>
    public MorePage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Overrides page appearance behavior to refresh user-specific UI state:
    /// - Loads saved avatar for current user
    /// - Updates displayed username from account context
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadAvatar();
        UsernameLabel.Text = AccountContext.CurrentUsername;
    }

    // Avatar Management
    /// <summary>
    /// Loads and applies the saved avatar for the current user:
    /// 1. Retrieves avatar name from user-scoped preferences
    /// 2. Applies the avatar to UI image element
    /// 3. Highlights the selected avatar in the selection UI
    /// </summary>
    private void LoadAvatar()
    {
        string key = $"{AvatarKeyPrefix}{AccountContext.CurrentUsername}";
        string saved = Preferences.Get(key, "avatar_dog"); // Default: dog avatar
        ApplyAvatar(saved);
        HighlightSelectedAvatar(saved);
    }

    /// <summary>
    /// Updates the main avatar image source with the specified avatar asset
    /// </summary>
    /// <param name="avatarName">Name of the avatar asset (without file extension)</param>
    private void ApplyAvatar(string avatarName)
    {
        AvatarImage.Source = $"{avatarName}.svg";
    }

    /// <summary>
    /// Visual highlight for selected avatar in the avatar selection popup:
    /// - Active avatar: Cyan border (#00FFFF)
    /// - Inactive avatars: Dark purple border (#3A3A5A)
    /// </summary>
    /// <param name="avatarName">Name of the selected avatar to highlight</param>
    private void HighlightSelectedAvatar(string avatarName)
    {
        DogBorder.Stroke = avatarName == "avatar_dog" ? Color.FromArgb("#00FFFF") : Color.FromArgb("#3A3A5A");
        RabbitBorder.Stroke = avatarName == "avatar_rabbit" ? Color.FromArgb("#00FFFF") : Color.FromArgb("#3A3A5A");
        CatBorder.Stroke = avatarName == "avatar_cat" ? Color.FromArgb("#00FFFF") : Color.FromArgb("#3A3A5A");
    }

    /// <summary>
    /// Persists selected avatar to user-scoped preferences and updates UI:
    /// 1. Saves avatar name to preferences
    /// 2. Updates main avatar display
    /// 3. Updates selection highlight state
    /// </summary>
    /// <param name="avatarName">Name of the avatar to save/apply</param>
    private void SaveAvatar(string avatarName)
    {
        string key = $"{AvatarKeyPrefix}{AccountContext.CurrentUsername}";
        Preferences.Set(key, avatarName);
        ApplyAvatar(avatarName);
        HighlightSelectedAvatar(avatarName);
    }

    /// <summary>Triggers display of the avatar selection popup</summary>
    /// <param name="sender">Gesture recognizer source (avatar border)</param>
    /// <param name="e">Tap gesture event arguments</param>
    private void OnAvatarTapped(object sender, EventArgs e) => ShowPopup(AvatarPopup);

    /// <summary>Selects dog avatar, saves to preferences, and closes popup</summary>
    /// <param name="sender">Dog avatar selection element</param>
    /// <param name="e">Tap gesture event arguments</param>
    private void OnSelectDog(object sender, EventArgs e) { SaveAvatar("avatar_dog"); ClosePopup(); }

    /// <summary>Selects rabbit avatar, saves to preferences, and closes popup</summary>
    /// <param name="sender">Rabbit avatar selection element</param>
    /// <param name="e">Tap gesture event arguments</param>
    private void OnSelectRabbit(object sender, EventArgs e) { SaveAvatar("avatar_rabbit"); ClosePopup(); }

    /// <summary>Selects cat avatar, saves to preferences, and closes popup</summary>
    /// <param name="sender">Cat avatar selection element</param>
    /// <param name="e">Tap gesture event arguments</param>
    private void OnSelectCat(object sender, EventArgs e) { SaveAvatar("avatar_cat"); ClosePopup(); }

    //  Software Settings Management 
    /// <summary>
    /// Prepares and displays the software settings popup:
    /// 1. Retrieves saved decimal precision and date format from preferences
    /// 2. Pre-selects corresponding values in picker controls
    /// 3. Shows the settings popup overlay
    /// </summary>
    /// <param name="sender">Settings card tap source</param>
    /// <param name="e">Tap gesture event arguments</param>
    private void OnSettingsClicked(object sender, EventArgs e)
    {
        string decKey = $"{DecimalKeyPrefix}{AccountContext.CurrentUsername}";
        string dfKey = $"{DateFormatKeyPrefix}{AccountContext.CurrentUsername}";
        DecimalPicker.SelectedIndex = Preferences.Get(decKey, 2); // Default: 2 decimal places
        DateFormatPicker.SelectedIndex = Preferences.Get(dfKey, 0);  // Default: yyyy-MM-dd
        ShowPopup(SettingsPopup);
    }

    /// <summary>
    /// Validates and saves software settings to user-scoped preferences:
    /// 1. Validates all picker selections are made
    /// 2. Saves decimal precision and date format to preferences
    /// 3. Closes popup and notifies user (restart required for changes)
    /// </summary>
    /// <param name="sender">Save button in settings popup</param>
    /// <param name="e">Button click event arguments</param>
    private async void OnSaveSettings(object sender, EventArgs e)
    {
        // Validate complete selection
        if (DecimalPicker.SelectedIndex < 0 || DateFormatPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Tip", "Please select all options before saving.", "OK");
            return;
        }

        // Persist settings
        string decKey = $"{DecimalKeyPrefix}{AccountContext.CurrentUsername}";
        string dfKey = $"{DateFormatKeyPrefix}{AccountContext.CurrentUsername}";
        Preferences.Set(decKey, DecimalPicker.SelectedIndex);
        Preferences.Set(dfKey, DateFormatPicker.SelectedIndex);

        // Cleanup and feedback
        ClosePopup();
        await DisplayAlert("Saved", "Settings saved. Restart the app to apply changes.", "OK");
    }

    // Data Management Operations 
    /// <summary>Triggers display of the data management popup</summary>
    /// <param name="sender">Data management card tap source</param>
    /// <param name="e">Tap gesture event arguments</param>
    private void OnDataManagementClicked(object sender, EventArgs e) => ShowPopup(DataPopup);

    /// <summary>
    /// Exports all user financial data to clipboard:
    /// 1. Builds structured text output with total amount, bills, and transactions
    /// 2. Copies formatted data to system clipboard
    /// 3. Provides success/error feedback to user
    /// </summary>
    /// <param name="sender">Export data tap source</param>
    /// <param name="e">Tap gesture event arguments</param>
    private async void OnExportData(object sender, EventArgs e)
    {
        var sb = new StringBuilder();
        // Header with account identifier
        sb.AppendLine($"=== FutureBound Export — {AccountContext.CurrentUsername} ===");
        sb.AppendLine($"Total Amount : ¥{AccountDataManager.LoadTotalAmount():F2}");
        sb.AppendLine();

        // Export bill data
        var bills = AccountDataManager.LoadBills();
        sb.AppendLine($"Bills ({bills.Count}):");
        foreach (var b in bills)
            sb.AppendLine($"  [{b.TypeLogo}] {b.Name} | ¥{b.CurrentAmount:F2} | {b.EventDate}");

        // Export transaction data
        sb.AppendLine();
        var txns = TransactionManager.Instance.Transactions;
        sb.AppendLine($"Transactions ({txns.Count}):");
        foreach (var t in txns)
            sb.AppendLine($"  {t.Time}  {(t.IsIncome ? "+" : "-")}{t.Amount}  {t.Remark}");

        // Attempt clipboard copy with error handling
        try
        {
            await Clipboard.Default.SetTextAsync(sb.ToString());
            ClosePopup();
            await DisplayAlert("Exported", "All data has been copied to your clipboard.", "OK");
        }
        catch
        {
            await DisplayAlert("Error", "Failed to copy to clipboard.", "OK");
        }
    }

    /// <summary>
    /// Securely clears all local financial data for the current user:
    /// 1. Requests user confirmation (irreversible action)
    /// 2. Deletes account-specific bill data
    /// 3. Clears in-memory transaction records
    /// 4. Provides completion feedback
    /// </summary>
    /// <param name="sender">Clear data tap source</param>
    /// <param name="e">Tap gesture event arguments</param>
    private async void OnClearCache(object sender, EventArgs e)
    {
        // Critical action confirmation
        bool confirm = await DisplayAlert(
            "Clear All Data",
            $"This will permanently delete all data for account \"{AccountContext.CurrentUsername}\". This cannot be undone.",
            "Delete", "Cancel");

        if (!confirm) return;

        // Execute data deletion
        AccountDataManager.DeleteAccountData(AccountContext.CurrentUsername);
        TransactionManager.Instance.Transactions.Clear();

        // Cleanup and feedback
        ClosePopup();
        await DisplayAlert("Done", "All local data has been cleared.", "OK");
    }

    //  About Information 
    /// <summary>
    /// Displays application information dialog with:
    /// - Version number
    /// - Core functionality description
    /// - Usage guide for main features
    /// - Data storage disclosure (local-only)
    /// </summary>
    /// <param name="sender">About card tap source</param>
    /// <param name="e">Tap gesture event arguments</param>
    private async void OnAboutClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "ℹ️  About FutureBound",
            "Version: 1.0.0\n\n" +
            "FutureBound is a personal finance manager designed for single-device use.\n\n" +
            "Quick Guide:\n" +
            "• Home  — Record income and expenses\n" +
            "• Bill   — Create and manage bill categories\n" +
            "• More — Settings and account management\n\n" +
            "All data is stored locally on your device.",
            "OK");
    }

    //  Account Switching 
    /// <summary>
    /// Prepares and displays account switching popup:
    /// 1. Retrieves list of registered accounts from preferences
    /// 2. Removes current account (prevents self-selection)
    /// 3. Validates non-empty account list
    /// 4. Populates account list view and shows popup
    /// </summary>
    /// <param name="sender">Account switching card tap source</param>
    /// <param name="e">Tap gesture event arguments</param>
    private void OnAccountSwitchClicked(object sender, EventArgs e)
    {
        // Load registered accounts
        string saved = Preferences.Get(RegisteredAccountsKey, "");
        var accounts = string.IsNullOrEmpty(saved)
            ? new List<string>()
            : saved.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToList();

        // Exclude current active account
        accounts.Remove(AccountContext.CurrentUsername);

        // Validate available accounts
        if (accounts.Count == 0)
        {
            DisplayAlert("No Other Accounts",
                "There are no other registered accounts on this device.", "OK");
            return;
        }

        // Prepare and show account selection UI
        AccountListView.ItemsSource = new ObservableCollection<string>(accounts);
        AccountListView.SelectedItem = null;
        ShowPopup(AccountSwitchPopup);
    }

    /// <summary>
    /// Processes account selection from the switching popup:
    /// 1. Validates selected account value
    /// 2. Requests user confirmation
    /// 3. Updates account context to selected user
    /// 4. Reinitializes transaction manager for new account
    /// 5. Navigates to home page with new account context
    /// </summary>
    /// <param name="sender">Account list view selection source</param>
    /// <param name="e">Selection changed event arguments</param>
    private async void OnAccountSelected(object sender, SelectionChangedEventArgs e)
    {
        // Validate selection
        if (e.CurrentSelection.FirstOrDefault() is not string selectedAccount) return;

        // Confirm account switch
        bool confirm = await DisplayAlert(
            "Switch Account",
            $"Switch to account \"{selectedAccount}\"?",
            "Switch", "Cancel");

        if (!confirm)
        {
            AccountListView.SelectedItem = null;
            return;
        }

        // Execute account switch
        AccountContext.CurrentUsername = selectedAccount;
        TransactionManager.Instance.Initialize();

        // Cleanup and navigation
        ClosePopup();
        await Navigation.PopToRootAsync(false);
        await Navigation.PushAsync(new HomePage(), false);
    }

    // Log Out Functionality 
    /// <summary>
    /// Securely logs out the current user:
    /// 1. Requests user confirmation (data remains saved locally)
    /// 2. Clears account context (unsets current username)
    /// 3. Navigates back to account selection root page
    /// </summary>
    /// <param name="sender">Log out card tap source</param>
    /// <param name="e">Tap gesture event arguments</param>
    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        // Logout confirmation
        bool confirm = await DisplayAlert(
            "Log Out",
            $"Log out of \"{AccountContext.CurrentUsername}\"?\nYour data will be saved.",
            "Log Out", "Cancel");

        if (!confirm) return;

        // Execute logout
        AccountContext.Clear();
        await Navigation.PopToRootAsync();
    }

    // Bottom Navigation 
    /// <summary>Navigates to the HomePage (income/expense tracking)</summary>
    /// <param name="sender">Home navigation button</param>
    /// <param name="e">Button click event arguments</param>
    private async void OnHomeClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(new HomePage());

    /// <summary>Navigates to the BillPage (bill category management)</summary>
    /// <param name="sender">Bill navigation button</param>
    /// <param name="e">Button click event arguments</param>
    private async void OnBillClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(new BillPage());

    // Popup Management Helpers
    /// <summary>
    /// Displays a specified popup with overlay:
    /// 1. Tracks active popup for targeted closing
    /// 2. Shows full-screen semi-transparent overlay
    /// 3. Makes target popup visible (centered)
    /// </summary>
    /// <param name="popup">Border component representing the popup to display</param>
    private void ShowPopup(Border popup)
    {
        _activePopup = popup;
        FullScreenOverlay.IsVisible = true;
        popup.IsVisible = true;
    }

    /// <summary>
    /// Closes the currently active popup:
    /// 1. Hides active popup if exists
    /// 2. Clears active popup tracking reference
    /// 3. Hides full-screen overlay
    /// </summary>
    private void ClosePopup()
    {
        if (_activePopup != null)
        {
            _activePopup.IsVisible = false;
            _activePopup = null;
        }
        FullScreenOverlay.IsVisible = false;
    }

    /// <summary>Closes popup when overlay is tapped (user dismiss action)</summary>
    /// <param name="sender">Full-screen overlay tap source</param>
    /// <param name="e">Tap gesture event arguments</param>
    private void OnOverlayTapped(object sender, EventArgs e) => ClosePopup();

    /// <summary>Closes popup when cancel/close button is clicked</summary>
    /// <param name="sender">Cancel/close button source</param>
    /// <param name="e">Button click event arguments</param>
    private void OnClosePopup(object sender, EventArgs e) => ClosePopup();
}