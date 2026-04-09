using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using FutureBound.Models;
using FutureBound.Pages;
using FutureBound.Data; // Added reference, style unchanged

namespace FutureBound.Page;

/// <summary>
/// BillPage - Main bill management page
/// Features:
/// - Display and filter bill list by type
/// - Create new bills with type selection
/// - Persist bill data to local storage
/// - Navigate to bill details, home, and more pages
/// - Show/hide new bill creation popup
/// </summary>
public partial class BillPage : ContentPage, INotifyPropertyChanged
{
    /// <summary>
    /// Full collection of bills (unfiltered)
    /// </summary>
    public ObservableCollection<Bill> Bills { get; set; }
    /// <summary>
    /// Filtered collection of bills for UI display
    /// </summary>
    public ObservableCollection<Bill> FilteredBills { get; set; }
    /// <summary>
    /// Bound to new bill name input field in popup
    /// </summary>
    public string NewBillName { get; set; }

    /// <summary>
    /// Initialize BillPage and load persisted bills
    /// Sets up filter picker and initial bill list
    /// </summary>
    public BillPage()
    {
        InitializeComponent();
        // Load persisted bills (only add this line, style unchanged)
        Bills = Data.AccountDataManager.LoadBills();
        FilteredBills = new ObservableCollection<Bill>();
        BindingContext = this;
        FilterPicker.Items.Add("All");
        FilterPicker.Items.Add("Travel");
        FilterPicker.Items.Add("Traffic");
        FilterPicker.Items.Add("Life");
        FilterPicker.SelectedIndex = 0;
        ApplyFilter(); //  Added: Filter after loading
    }

    /// <summary>
    /// Handle add new bill button click - show popup and overlay
    /// </summary>
    /// <param name="sender">Button that triggered the event</param>
    /// <param name="e">Event arguments</param>
    private void OnAddBillClicked(object sender, EventArgs e)
    {
        FullScreenOverlay.IsVisible = true;
        NewBillPopup.IsVisible = true;
    }

    /// <summary>
    /// Handle cancel new bill button click - hide popup and overlay
    /// Resets input fields and picker selection
    /// </summary>
    /// <param name="sender">Button that triggered the event</param>
    /// <param name="e">Event arguments</param>
    private void OnCancelNewBill(object sender, EventArgs e)
    {
        NewBillPopup.IsVisible = false;
        FullScreenOverlay.IsVisible = false;
        NewBillName = string.Empty;
        TypePicker.SelectedIndex = -1;
        EventDatePicker.Date = DateTime.Today;
    }

    /// <summary>
    /// Handle submit new bill button click - create new bill and save
    /// Validates input, creates bill object, saves to storage, and updates UI
    /// </summary>
    /// <param name="sender">Button that triggered the event</param>
    /// <param name="e">Event arguments</param>
    private void OnSubmitNewBill(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NewBillName) || TypePicker.SelectedItem == null)
        {
            DisplayAlert("Tip", "Please fill in all fields", "OK");
            return;
        }
        string type = TypePicker.SelectedItem.ToString();
        Color color = type == "Travel" ? Colors.LightGreen : type == "Traffic" ? Colors.LightCoral : Colors.LightSkyBlue;
        string logo = type == "Travel" ? "✈️" : type == "Traffic" ? "🚗" : "🛒";

        Bills.Add(new Bill
        {
            Name = NewBillName,
            Type = type,
            LastModifiedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            // Only change here: Assign TypeColorHex instead of TypeColor
            TypeColorHex = ToHex(color),
            TypeLogo = logo,
            CurrentAmount = 0,
            EventDate = $"{EventDatePicker.Date:yyyy-MM-dd}"
        });

        // Save bills to local storage (style unchanged)
        Data.AccountDataManager.SaveBills(Bills);

        NewBillPopup.IsVisible = false;
        FullScreenOverlay.IsVisible = false;
        NewBillName = string.Empty;
        TypePicker.SelectedIndex = -1;
        EventDatePicker.Date = DateTime.Today;
        ApplyFilter();
    }

    /// <summary>
    /// Utility method: Convert Color object to hex string
    /// Used for persisting bill type colors
    /// </summary>
    /// <param name="c">Color to convert</param>
    /// <returns>Hex string in format #RRGGBB</returns>
    private string ToHex(Color c)
    {
        return $"#{(int)(c.Red * 255):X2}{(int)(c.Green * 255):X2}{(int)(c.Blue * 255):X2}";
    }

    /// <summary>
    /// Handle filter picker selection change - apply bill filter
    /// </summary>
    /// <param name="sender">Picker that triggered the event</param>
    /// <param name="e">Event arguments</param>
    private void OnFilterSelected(object sender, EventArgs e) => ApplyFilter();

    /// <summary>
    /// Apply filter to bill list based on selected filter type
    /// Updates FilteredBills collection with matching bills
    /// </summary>
    private void ApplyFilter()
    {
        FilteredBills.Clear();
        string f = FilterPicker.SelectedItem.ToString();
        foreach (var b in Bills)
            if (f == "All" || b.Type == f)
                FilteredBills.Add(b);
    }

    /// <summary>
    /// Handle bill card tap - navigate to BillDetailPage for selected bill
    /// </summary>
    /// <param name="sender">Border control that triggered the tap</param>
    /// <param name="e">Tap event arguments with bill parameter</param>
    private async void OnBillTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Bill bill)
            await Navigation.PushAsync(new BillDetailPage(bill));
    }

    /// <summary>
    /// Handle home button click - navigate to HomePage
    /// </summary>
    /// <param name="sender">Button that triggered the event</param>
    /// <param name="e">Event arguments</param>
    private async void OnHomeClicked(object sender, EventArgs e) => await Navigation.PushAsync(new HomePage());

    /// <summary>
    /// Handle more button click - navigate to MorePage
    /// </summary>
    /// <param name="sender">Button that triggered the event</param>
    /// <param name="e">Event arguments</param>
    private async void OnMoreClicked(object sender, EventArgs e) => await Navigation.PushAsync(new MorePage());

    /// <summary>
    /// Property changed event for data binding
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
}
