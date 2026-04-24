using FutureBound.Data;
using FutureBound.Page;
using FutureBound.Pages;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Devices;

namespace FutureBound.Pages;

/// <summary>
/// Main application homepage (HomePage)
/// Core Responsibilities:
/// - Display total financial balance with smooth animated updates
/// - Handle income (Save) and expense (Cost) transactions
/// - Navigate to secondary pages (FlowRecord, Bill, More)
/// - Provide haptic feedback for successful transactions
/// - Persist balance data to local storage
/// - Send system notifications for transaction confirmations
/// </summary>
public partial class HomePage : ContentPage
{
    /// <summary>
    /// Current total financial balance (persisted to local storage)
    /// </summary>
    private decimal _totalAmount = 0;

    /// <summary>
    /// Initializes HomePage components and loads saved balance
    /// </summary>
    /// <remarks>
    /// - Initializes TransactionManager for transaction tracking
    /// - Loads previously saved balance using AccountDataManager
    /// - Updates UI with saved balance value
    /// </remarks>
    public HomePage()
    {
        InitializeComponent();
        TransactionManager.Instance.Initialize();
        decimal saved = AccountDataManager.LoadTotalAmount();
        UpdateTotalAmountDisplay(saved);
    }

    // ✅ 每次页面出现时用最新格式刷新金额显示（从设置页返回后立即生效）
    protected override void OnAppearing()
    {
        base.OnAppearing();
        string fmt = AccountDataManager.GetAmountFormat();
        if (TotalAmountLabel != null)
            TotalAmountLabel.Text = $"¥ {_totalAmount.ToString(fmt)}";
    }

    /// <summary>
    /// Updates balance display with smooth 500ms animation and persists new value
    /// </summary>
    private async void UpdateTotalAmountDisplay(decimal newAmount)
    {
        decimal oldAmount = _totalAmount;
        _totalAmount = newAmount;
        AccountDataManager.SaveTotalAmount(newAmount);

        // ✅ 读取用户设置的小数位数，不再硬编码 F2
        string fmt = AccountDataManager.GetAmountFormat();

        for (int i = 0; i <= 20; i++)
        {
            decimal progress = (decimal)i / 20;
            decimal current = oldAmount + (newAmount - oldAmount) * progress;
            if (TotalAmountLabel != null)
                TotalAmountLabel.Text = $"¥ {current.ToString(fmt)}";
            await Task.Delay(25);
        }
        if (TotalAmountLabel != null)
            TotalAmountLabel.Text = $"¥ {newAmount.ToString(fmt)}";
    }

    /// <summary>
    /// Handles Flow Record button click - navigates to FlowRecordPage
    /// </summary>
    /// <param name="sender">Flow Record button control</param>
    /// <param name="e">Button click event arguments</param>
    private async void OnFlowRecordClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FlowRecordPage());
    }

    /// <summary>
    /// Handles Bill button click - navigates to BillPage
    /// </summary>
    /// <param name="sender">Bill button control</param>
    /// <param name="e">Button click event arguments</param>
    private async void OnBillClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new BillPage());
    }

    /// <summary>
    /// Handles More button click - navigates to MorePage
    /// </summary>
    /// <param name="sender">More button control</param>
    /// <param name="e">Button click event arguments</param>
    private async void OnMoreClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MorePage());
    }

    /// <summary>
    /// Handles Save (income) button click - shows custom popup for amount/remark input
    /// Creates income transaction, updates balance, and sends confirmation notification
    /// </summary>
    /// <param name="sender">Save button control</param>
    /// <param name="e">Button click event arguments</param>
    /// <remarks>
    /// Validation Rules:
    /// - Amount must be a valid decimal greater than 0
    /// - Empty remark defaults to "No remark"
    /// 
    /// Post-validation Actions:
    /// - Updates total balance with animation
    /// - Adds transaction to TransactionManager
    /// - Triggers haptic vibration feedback
    /// - Sends success notification via NotificationHelper
    /// - Closes input popup
    /// 
    /// Error Handling:
    /// - Shows error alert for invalid/non-positive amounts
    /// </remarks>
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

        // Create save transaction popup UI with styled components
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

        // Get button row and attach click handlers for Cancel/Submit
        var btnRow = (HorizontalStackLayout)popup.Children[3];
        ((Button)btnRow.Children[0]).Clicked += async (s, a) => await Navigation.PopModalAsync();
        ((Button)btnRow.Children[1]).Clicked += async (s, a) =>
        {
            // Validate amount input (must be positive decimal)
            if (decimal.TryParse(amountEntry.Text, out var amt) && amt > 0)
            {
                // Update total balance with animation
                UpdateTotalAmountDisplay(_totalAmount + amt);

                // Create new income transaction record
                var transaction = new Transaction
                {
                    Icon = "💾",
                    Remark = remarkEntry.Text ?? "No remark",
                    Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    Amount = $"+{amt:F2}",
                    IsIncome = true
                };
                TransactionManager.Instance.AddTransaction(transaction);

                // Trigger haptic feedback for successful transaction
                TryVibrate();

                // Send system notification for successful save operation
                NotificationHelper.SendNotification(
                    NotificationHelper.HomeSaveId,
                    "Saved! 💾",
                    $"¥{amt:F2} added to your balance",
                    NotificationHelper.ReturnToHome);

                // Close the input popup
                await Navigation.PopModalAsync();
            }
            else
            {
                // Show error alert for invalid amount input
                await DisplayAlertAsync("Error", "Please enter a valid amount", "OK");
            }
        };

        // Display popup with dimmed background overlay
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
    /// Handles Cost (expense) button click - shows custom popup for amount/remark input
    /// Creates expense transaction, updates balance (if sufficient funds), sends confirmation notification
    /// </summary>
    /// <param name="sender">Cost button control</param>
    /// <param name="e">Button click event arguments</param>
    /// <remarks>
    /// Validation Rules:
    /// - Amount must be valid decimal > 0
    /// - Current balance must be ≥ expense amount
    /// - Empty remark defaults to "No remark"
    /// 
    /// Post-validation Actions:
    /// - Deducts amount from total balance (with animation)
    /// - Adds expense transaction to TransactionManager
    /// - Triggers haptic vibration feedback
    /// - Sends success notification via NotificationHelper
    /// - Closes input popup
    /// 
    /// Error Handling:
    /// - Shows error alert for invalid amount or insufficient balance
    /// </remarks>
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

        // Create cost transaction popup UI with styled components
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

        // Get button row and attach click handlers for Cancel/Submit
        var btnRow = (HorizontalStackLayout)popup.Children[3];
        ((Button)btnRow.Children[0]).Clicked += async (s, a) => await Navigation.PopModalAsync();
        ((Button)btnRow.Children[1]).Clicked += async (s, a) =>
        {
            // Validate amount input and sufficient balance
            if (decimal.TryParse(amountEntry.Text, out var amt) && amt > 0 && _totalAmount >= amt)
            {
                // Update total balance with animation (deduct expense)
                UpdateTotalAmountDisplay(_totalAmount - amt);

                // Create new expense transaction record
                var transaction = new Transaction
                {
                    Icon = "💸",
                    Remark = remarkEntry.Text ?? "No remark",
                    Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    Amount = $"-{amt:F2}",
                    IsIncome = false
                };
                TransactionManager.Instance.AddTransaction(transaction);

                // Trigger haptic feedback for successful transaction
                TryVibrate();

                // Send system notification for successful cost operation
                NotificationHelper.SendNotification(
                    NotificationHelper.HomeCostId,
                    "Cost! 💸",
                    $"¥{amt:F2} deducted from your balance",
                    NotificationHelper.ReturnToHome);

                // Close the input popup
                await Navigation.PopModalAsync();
            }
            else
            {
                // Show error alert for invalid amount or insufficient balance
                await DisplayAlertAsync("Error", "Invalid amount or insufficient balance", "OK");
            }
        };

        // Display popup with dimmed background overlay
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
    /// Wrapper method for DisplayAlert to maintain consistent async pattern
    /// Overrides base method to return Task instead of void
    /// </summary>
    /// <param name="title">Alert dialog title</param>
    /// <param name="message">Alert dialog content message</param>
    /// <param name="cancel">Text for cancel/OK button</param>
    /// <returns>Task representing the async alert operation</returns>
    private new Task DisplayAlertAsync(string title, string message, string cancel)
    {
        return DisplayAlert(title, message, cancel);
    }

    /// <summary>
    /// Triggers short haptic vibration feedback (100ms) if supported by device
    /// </summary>
    /// <remarks>
    /// Error Handling:
    /// - Catches all exceptions to prevent app crashes
    /// - Silently fails if vibration is unsupported or unavailable
    /// - Does not affect main application flow
    /// </remarks>
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
            // Ignore vibration exceptions to not impact core functionality
        }
    }
}