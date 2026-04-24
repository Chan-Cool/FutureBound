using Microsoft.Maui.Controls;
using FutureBound.Models;
using FutureBound.Data;
using Microsoft.Maui.Devices;

namespace FutureBound.Page;

/// <summary>
/// Bill detail page that displays bill information, balance, records, and adjustment actions
/// Allows users to increase or decrease the bill amount with validation
/// </summary>
public partial class BillDetailPage : ContentPage
{
    /// <summary>
    /// Current bill instance being displayed and edited
    /// </summary>
    public Bill Bill { get; set; }

    /// <summary>
    /// Constructor that accepts a Bill object and initializes the page UI
    /// </summary>
    /// <param name="bill">The bill to display details for</param>
    public BillDetailPage(Bill bill)
    {
        InitializeComponent();
        Bill = bill;
        BindingContext = Bill;
    }

    /// <summary>
    /// Triggered when the user clicks the Increase button to add funds to the bill
    /// Includes input validation, amount update, record creation, and persistence
    /// </summary>
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Prompt user to enter the amount to increase
        var amt = await DisplayPromptAsync("Increase", "Enter Amount", keyboard: Keyboard.Numeric);
        if (amt == null) return;

        // Validate input: must be a valid positive decimal number
        if (!decimal.TryParse(amt, out var num) || num <= 0)
        {
            await DisplayAlert("Error", "Please enter a valid positive number", "OK");
            return;
        }

        // Prompt user to enter a remark for this transaction
        var rem = await DisplayPromptAsync("Increase", "Remark");

        // Update bill balance and record modification time
        Bill.CurrentAmount += num;
        // ✅ 读取用户设置的日期格式
        string dateFormatSave = Data.AccountDataManager.GetDateFormat();
        Bill.LastModifiedTime = DateTime.Now.ToString($"{dateFormatSave} HH:mm");

        // Add a new transaction record of type Increase (IsSave = true)
        Bill.Records.Add(new BillRecord
        {
            Amount = num,
            Remark = rem,
            IsSave = true,
            Time = DateTime.Now.ToString("HH:mm")
        });

        // Get the parent BillPage to update the overall bill list
        var billPage = (BillPage)Navigation.NavigationStack[Navigation.NavigationStack.Count - 2];
        // Save updated bill list to persistent storage
        Data.AccountDataManager.SaveBills(billPage.Bills);

        // Provide haptic feedback
        TryVibrate();

        // Send system notification for successful increase
        NotificationHelper.SendNotification(
            NotificationHelper.BillSaveId,
            "Bill Increased! 💰",
            $"Added ¥{num:F2} to {Bill.Name}",
            NotificationHelper.ReturnToBill);
    }

    /// <summary>
    /// Triggered when the user clicks the Decrease button to deduct funds from the bill
    /// Includes validation for positive amount and sufficient balance
    /// </summary>
    private async void OnCostClicked(object sender, EventArgs e)
    {
        // Prompt user to enter the amount to decrease
        var amt = await DisplayPromptAsync("Decrease", "Enter Amount", keyboard: Keyboard.Numeric);
        if (amt == null) return;

        // Validate input: must be a valid positive decimal number
        if (!decimal.TryParse(amt, out var num) || num <= 0)
        {
            await DisplayAlert("Error", "Please enter a valid positive number", "OK");
            return;
        }

        // Validate sufficient balance before deduction
        if (Bill.CurrentAmount < num)
        {
            await DisplayAlert("Error", "Insufficient balance", "OK");
            return;
        }

        // Prompt user to enter a remark for this transaction
        var rem = await DisplayPromptAsync("Decrease", "Remark");

        // Deduct amount from bill balance and update time
        Bill.CurrentAmount -= num;
        // ✅ 读取用户设置的日期格式
        string dateFormatCost = Data.AccountDataManager.GetDateFormat();
        Bill.LastModifiedTime = DateTime.Now.ToString($"{dateFormatCost} HH:mm");

        // Add a new transaction record of type Decrease (IsSave = false)
        Bill.Records.Add(new BillRecord
        {
            Amount = num,
            Remark = rem,
            IsSave = false,
            Time = DateTime.Now.ToString("HH:mm")
        });

        // Get the parent BillPage to update the overall bill list
        var billPage = (BillPage)Navigation.NavigationStack[Navigation.NavigationStack.Count - 2];
        // Save updated bill list to persistent storage
        Data.AccountDataManager.SaveBills(billPage.Bills);

        // Provide haptic feedback
        TryVibrate();

        // Send system notification for successful decrease
        NotificationHelper.SendNotification(
            NotificationHelper.BillCostId,
            "Bill Decreased! 💸",
            $"Deducted ¥{num:F2} from {Bill.Name}",
            NotificationHelper.ReturnToBill);
    }

    /// <summary>
    /// Navigate back to the previous page (BillPage)
    /// </summary>
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    /// <summary>
    /// Safely trigger device vibration for user feedback
    /// Includes exception handling to avoid crashes on unsupported devices
    /// </summary>
    private void TryVibrate()
    {
        try
        {
            if (Vibration.Default.IsSupported)
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(100));
        }
        catch (Exception) { }
    }
}