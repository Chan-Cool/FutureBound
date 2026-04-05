using FutureBound.Data;
using FutureBound.Page;
using FutureBound.Pages;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using System;
using System.Threading.Tasks;
// Added: Import vibration API namespace
using Microsoft.Maui.Devices;

namespace FutureBound.Pages;

/// <summary>
/// HomePage - Main application homepage
/// Core features:
/// - Display total financial balance with animated updates
/// - Add income (Save) and expense (Cost) transactions
/// - Navigate to FlowRecordPage, BillPage, and MorePage
/// - Provide haptic feedback on successful transactions
/// - Persist total balance to local storage
/// </summary>
public partial class HomePage : ContentPage
{
    /// <summary>
    /// Current total financial balance
    /// </summary>
    private decimal _totalAmount = 0;

    /// <summary>
    /// Initialize HomePage and load saved total balance
    /// Sets up transaction manager and initial UI state
    /// </summary>
    public HomePage()
    {
        InitializeComponent();
        TransactionManager.Instance.Initialize();
        decimal saved = AccountDataManager.LoadTotalAmount();
        UpdateTotalAmountDisplay(saved);
    }

    /// <summary>
    /// Update total balance display with smooth animation
    /// Saves new balance to local storage and animates value transition
    /// </summary>
    /// <param name="newAmount">New total balance to display</param>
    private async void UpdateTotalAmountDisplay(decimal newAmount)
    {
        decimal oldAmount = _totalAmount;
        _totalAmount = newAmount;
        AccountDataManager.SaveTotalAmount(newAmount);

        // Animate balance transition over 500ms (20 steps × 25ms)
        for (int i = 0; i <= 20; i++)
        {
            decimal progress = (decimal)i / 20;
            decimal current = oldAmount + (newAmount - oldAmount) * progress;
            if (TotalAmountLabel != null)
                TotalAmountLabel.Text = $"¥ {current:F2}";
            await Task.Delay(25);
        }
        // Ensure final value is set correctly
        if (TotalAmountLabel != null)
            TotalAmountLabel.Text = $"¥ {newAmount:F2}";
    }

    /// <summary>
    /// Handle Flow Record button click - navigate to FlowRecordPage
    /// </summary>
    /// <param name="sender">Button that triggered the event</param>
    /// <param name="e">Event arguments</param>
    private async void OnFlowRecordClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FlowRecordPage());
    }

    /// <summary>
    /// Handle Bill button click - navigate to BillPage
    /// </summary>
    /// <param name="sender">Button that triggered the event</param>
    /// <param name="e">Event arguments</param>
    private async void OnBillClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new BillPage());
    }

    /// <summary>
    /// Handle More button click - navigate to MorePage
    /// </summary>
    /// <param name="sender">Button that triggered the event</param>
    /// <param name="e">Event arguments</param>
    private async void OnMoreClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MorePage());
    }

    /// <summary>
    /// Handle Save (income) button click - show popup for amount/remark input
    /// Creates new income transaction and updates total balance
    /// </summary>
    /// <param name="sender">Button that triggered the event</param>
    /// <param name="e">Event arguments</param>
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var amountEntry = new Entry
        {
            Placeholder = "Enter amount",
            Keyboard = Keyboard.Numeric,
            TextColor = Colors.White,
            PlaceholderColor = Colors.LightGray,
            FontSize = 22
        };
        var remarkEntry = new Entry
        {
            Placeholder = "Enter remark",
            TextColor = Colors.White,
            PlaceholderColor = Colors.LightGray,
            FontSize = 22
        };

        var amountBorder = new Border
        {
            Content = amountEntry,
            BackgroundColor = Color.FromArgb("#1A1A3A"),
            Padding = new Thickness(10),
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(10) },
            Margin = new Thickness(0, 5)
        };
        var remarkBorder = new Border
        {
            Content = remarkEntry,
            BackgroundColor = Color.FromArgb("#1A1A3A"),
            Padding = new Thickness(10),
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(10) },
            Margin = new Thickness(0, 5)
        };

        // Create save transaction popup UI
        var popup = new VerticalStackLayout
        {
            BackgroundColor = Color.FromArgb("#1A1A3A").WithAlpha(0.95f),
            Padding = 30,
            Spacing = 25,
            Children =
            {
                new HorizontalStackLayout
                {
                    Spacing = 20,
                    Children =
                    {
                        new Border
                        {
                            Stroke = Color.FromArgb("#00BFFF"),
                            StrokeThickness = 3,
                            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(40) },
                            BackgroundColor = Color.FromArgb("#00BFFF").WithAlpha(0.2f),
                            Padding = 15,
                            WidthRequest = 80,
                            HeightRequest = 80,
                            Content = new Label { Text = "💾", FontSize = 32, TextColor = Colors.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }
                        },
                        new Label { Text = "Save", FontSize = 48, FontAttributes = FontAttributes.Bold, TextColor = Colors.White, VerticalOptions = LayoutOptions.Center }
                    }
                },
                new HorizontalStackLayout
                {
                    Spacing = 15,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label { Text = "Amount:", FontSize = 22, TextColor = Colors.White, VerticalOptions = LayoutOptions.Center },
                        amountBorder
                    }
                },
                new HorizontalStackLayout
                {
                    Spacing = 15,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label { Text = "Remark:", FontSize = 22, TextColor = Colors.White, VerticalOptions = LayoutOptions.Center },
                        remarkBorder
                    }
                },
                new HorizontalStackLayout
                {
                    Spacing = 40,
                    HorizontalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Button { Text = "Cancel", FontSize = 22, BackgroundColor = Colors.Gray, TextColor = Colors.White, Padding = 15, CornerRadius = 20 },
                        new Button { Text = "Submit", FontSize = 22, BackgroundColor = Color.FromArgb("#00BFFF"), TextColor = Colors.White, Padding = 15, CornerRadius = 20 }
                    }
                }
            }
        };

        // Get button row and attach click handlers
        var btnRow = (HorizontalStackLayout)popup.Children[3];
        ((Button)btnRow.Children[0]).Clicked += async (s, a) => await Navigation.PopModalAsync();
        ((Button)btnRow.Children[1]).Clicked += async (s, a) =>
        {
            // Validate amount input
            if (decimal.TryParse(amountEntry.Text, out var amt) && amt > 0)
            {
                // Update total balance
                UpdateTotalAmountDisplay(_totalAmount + amt);

                // Create new income transaction
                var transaction = new Transaction
                {
                    Icon = "💾",
                    Remark = remarkEntry.Text ?? "No remark",
                    Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    Amount = $"+{amt:F2}",
                    IsIncome = true
                };
                TransactionManager.Instance.AddTransaction(transaction);

                // ✅ Added: Vibrate on successful operation
                TryVibrate();

                // Close popup
                await Navigation.PopModalAsync();
            }
            else await DisplayAlertAsync("Error", "Please enter a valid amount", "OK");
        };

        // Display popup with dimmed background
        await Navigation.PushModalAsync(new ContentPage
        {
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.8),
            Content = new Border
            {
                Content = popup,
                Stroke = Color.FromArgb("#00FFFF"),
                StrokeThickness = 3,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(25) },
                Padding = 0,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 340
            }
        });
    }

    /// <summary>
    /// Handle Cost (expense) button click - show popup for amount/remark input
    /// Creates new expense transaction and updates total balance (if sufficient funds)
    /// </summary>
    /// <param name="sender">Button that triggered the event</param>
    /// <param name="e">Event arguments</param>
    private async void OnCostClicked(object sender, EventArgs e)
    {
        var amountEntry = new Entry
        {
            Placeholder = "Enter amount",
            Keyboard = Keyboard.Numeric,
            TextColor = Colors.White,
            PlaceholderColor = Colors.LightGray,
            FontSize = 20
        };
        var remarkEntry = new Entry
        {
            Placeholder = "Enter remark",
            TextColor = Colors.White,
            PlaceholderColor = Colors.LightGray,
            FontSize = 20
        };

        var amountBorder = new Border
        {
            Content = amountEntry,
            BackgroundColor = Color.FromArgb("#1A1A3A"),
            Padding = new Thickness(10),
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(10) },
            Margin = new Thickness(0, 5)
        };
        var remarkBorder = new Border
        {
            Content = remarkEntry,
            BackgroundColor = Color.FromArgb("#1A1A3A"),
            Padding = new Thickness(10),
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(10) },
            Margin = new Thickness(0, 5)
        };

        // Create cost transaction popup UI
        var popup = new VerticalStackLayout
        {
            BackgroundColor = Color.FromArgb("#1A1A3A").WithAlpha(0.95f),
            Padding = 30,
            Spacing = 25,
            Children =
            {
                new HorizontalStackLayout
                {
                    Spacing = 20,
                    Children =
                    {
                        new Border
                        {
                            Stroke = Color.FromArgb("#FF4500"),
                            StrokeThickness = 3,
                            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(40) },
                            BackgroundColor = Color.FromArgb("#FF4500").WithAlpha(0.2f),
                            Padding = 15,
                            WidthRequest = 80,
                            HeightRequest = 80,
                            Content = new Label { Text = "💸", FontSize = 32, TextColor = Colors.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }
                        },
                        new Label { Text = "Cost", FontSize = 48, FontAttributes = FontAttributes.Bold, TextColor = Colors.White, VerticalOptions = LayoutOptions.Center }
                    }
                },
                new HorizontalStackLayout
                {
                    Spacing = 15,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label { Text = "Amount:", FontSize = 20, TextColor = Colors.White, VerticalOptions = LayoutOptions.Center },
                        amountBorder
                    }
                },
                new HorizontalStackLayout
                {
                    Spacing = 15,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label { Text = "Remark:", FontSize = 20, TextColor = Colors.White, VerticalOptions = LayoutOptions.Center },
                        remarkBorder
                    }
                },
                new HorizontalStackLayout
                {
                    Spacing = 40,
                    HorizontalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Button { Text = "Cancel", FontSize = 22, BackgroundColor = Colors.Gray, TextColor = Colors.White, Padding = 15, CornerRadius = 20 },
                        new Button { Text = "Submit", FontSize = 22, BackgroundColor = Color.FromArgb("#FF4500"), TextColor = Colors.White, Padding = 15, CornerRadius = 20 }
                    }
                }
            }
        };

        // Get button row and attach click handlers
        var btnRow = (HorizontalStackLayout)popup.Children[3];
        ((Button)btnRow.Children[0]).Clicked += async (s, a) => await Navigation.PopModalAsync();
        ((Button)btnRow.Children[1]).Clicked += async (s, a) =>
        {
            // Validate amount input and sufficient balance
            if (decimal.TryParse(amountEntry.Text, out var amt) && amt > 0 && _totalAmount >= amt)
            {
                // Update total balance
                UpdateTotalAmountDisplay(_totalAmount - amt);

                // Create new expense transaction
                var transaction = new Transaction
                {
                    Icon = "💸",
                    Remark = remarkEntry.Text ?? "No remark",
                    Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    Amount = $"-{amt:F2}",
                    IsIncome = false
                };
                TransactionManager.Instance.AddTransaction(transaction);

                // ✅ Added: Vibrate on successful operation
                TryVibrate();

                // Close popup
                await Navigation.PopModalAsync();
            }
            else await DisplayAlertAsync("Error", "Invalid amount or insufficient balance", "OK");
        };

        // Display popup with dimmed background
        await Navigation.PushModalAsync(new ContentPage
        {
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.8),
            Content = new Border
            {
                Content = popup,
                Stroke = Color.FromArgb("#FF6347"),
                StrokeThickness = 3,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(25) },
                Padding = 0,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 340
            }
        });
    }

    /// <summary>
    /// Wrapper method for DisplayAlert to return Task (consistent async pattern)
    /// </summary>
    /// <param name="title">Alert title</param>
    /// <param name="message">Alert message</param>
    /// <param name="cancel">Cancel button text</param>
    /// <returns>Task for async operation</returns>
    private Task DisplayAlertAsync(string title, string message, string cancel)
    {
        return DisplayAlert(title, message, cancel);
    }

    /// <summary>
    /// Local vibration method for haptic feedback
    /// Triggers short vibration (100ms) if supported by the device
    /// Catches exceptions to prevent app crashes
    /// </summary>
    private void TryVibrate()
    {
        try
        {
            if (Vibration.Default.IsSupported)
            {
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(100));
            }
        }
        catch (Exception)
        {
            // Ignore vibration exceptions to not affect main flow
        }
    }
}
