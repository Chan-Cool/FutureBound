using Microsoft.Maui.Controls;
using FutureBound.Models;
using FutureBound.Data;
using Microsoft.Maui.Devices;

namespace FutureBound.Page;

/// <summary>
/// BillDetailPage - Manages detailed bill operations
/// Features: 
/// - Display bill information and transaction history
/// - Handle deposit (Save) and expense (Cost) operations
/// - Persist bill data changes to local storage
/// - Provide haptic feedback on successful operations
/// </summary>
public partial class BillDetailPage : ContentPage
{
    /// <summary>
    /// Current bill being managed
    /// </summary>
    public Bill Bill { get; set; }

    /// <summary>
    /// Initialize BillDetailPage with specified bill
    /// </summary>
    /// <param name="bill">Bill to display and manage</param>
    public BillDetailPage(Bill bill)
    {
        InitializeComponent();
        Bill = bill;
        BindingContext = Bill;
    }

    /// <summary>
    /// Handle Save (deposit) button click event
    /// Prompts user for amount and remark, then updates bill balance and records
    /// </summary>
    /// <param name="sender">Button that triggered the event</param>
    /// <param name="e">Event arguments</param>
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var amt = await DisplayPromptAsync("Save", "Enter Amount");
        var rem = await DisplayPromptAsync("Save", "Remark");
        if (decimal.TryParse(amt, out var num) && num > 0)
        {
            Bill.CurrentAmount += num;
            Bill.LastModifiedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            Bill.Records.Add(new BillRecord
            {
                Amount = num,
                Remark = rem,
                IsSave = true,
                Time = DateTime.Now.ToString("HH:mm")
            });

            // Save modifications to local storage
            var billPage = (BillPage)Navigation.NavigationStack[Navigation.NavigationStack.Count - 2];
            Data.AccountDataManager.SaveBills(billPage.Bills);

            //Vibration on successful operation (direct inline)
            TryVibrate();
            //Send bill deposit notice
            NotificationHelper.SendImmediateNotification("Bill Saved!", $"Added ¥{num:F2} to {Bill.Name}");
        }
    }

    /// <summary>
    /// Handle Cost (expense) button click event
    /// Prompts user for amount and remark, then updates bill balance and records (if sufficient funds)
    /// </summary>
    /// <param name="sender">Button that triggered the event</param>
    /// <param name="e">Event arguments</param>
    private async void OnCostClicked(object sender, EventArgs e)
    {
        var amt = await DisplayPromptAsync("Cost", "Enter Amount");
        var rem = await DisplayPromptAsync("Cost", "Remark");
        if (decimal.TryParse(amt, out var num) && num > 0 && Bill.CurrentAmount >= num)
        {
            Bill.CurrentAmount -= num;
            Bill.LastModifiedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            Bill.Records.Add(new BillRecord
            {
                Amount = num,
                Remark = rem,
                IsSave = false,
                Time = DateTime.Now.ToString("HH:mm")
            });

            // Save modifications to local storage
            var billPage = (BillPage)Navigation.NavigationStack[Navigation.NavigationStack.Count - 2];
            Data.AccountDataManager.SaveBills(billPage.Bills);

            // Vibration on successful operation (direct inline)
            TryVibrate();
            // Send bill expenditure notifications
            NotificationHelper.SendImmediateNotification("Bill Cost!", $"Deducted ¥{num:F2} from {Bill.Name}");
        }
    }

    /// <summary>
    /// Handle back button click event - navigate back to BillPage
    /// </summary>
    /// <param name="sender">Button that triggered the event</param>
    /// <param name="e">Event arguments</param>
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    /// <summary>
    /// Local vibration method for haptic feedback
    /// Triggers short vibration if supported by the device
    /// </summary>
    private void TryVibrate()
    {
        try
        {
            if (Vibration.Default.IsSupported)
            {
                // Short vibration (100ms) for successful operation feedback
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(100));
            }
        }
        catch (Exception)
        {
            // Ignore vibration exceptions to prevent app crashes
        }
    }
}
