using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls.Shapes;
using System;

namespace FutureBound.Pages;

public partial class HomePage : ContentPage
{
    private decimal _totalAmount = 0;

    public HomePage()
    {
        InitializeComponent();
        UpdateTotalAmountDisplay();
    }

    // 更新总额显示
    private void UpdateTotalAmountDisplay()
    {
        TotalAmountLabel.Text = $"¥ {_totalAmount:F2}";
    }

    // 右上角 Flow Record 跳转
    private async void OnFlowRecordClicked(object sender, EventArgs e)
    {
        // await Navigation.PushAsync(new FlowRecordPage());
        await DisplayAlert("提示", "跳转到流水记录页面", "确定");
    }

    // 底部 Bill 跳转
    private async void OnBillClicked(object sender, EventArgs e)
    {
        // await Navigation.PushAsync(new BillPage());
        await DisplayAlert("提示", "跳转到账单页面", "确定");
    }

    // 底部 More 跳转
    private async void OnMoreClicked(object sender, EventArgs e)
    {
        // await Navigation.PushAsync(new MorePage());
        await DisplayAlert("提示", "跳转到更多页面", "确定");
    }

    // 点击 Save 弹出弹窗
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var amountEntry = new Entry { Placeholder = "input", Keyboard = Keyboard.Numeric };
        var remarkEntry = new Entry { Placeholder = "input" };

        var savePopup = new VerticalStackLayout
        {
            BackgroundColor = Color.FromArgb("#FAFAD2"),
            Padding = 20,
            Spacing = 15,
            Children =
            {
                new HorizontalStackLayout
                {
                    Spacing = 15,
                    Children =
                    {
                        new Border
                        {
                            // 适配 .NET 10.0 圆角新语法
                            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(50) },
                            BackgroundColor = Color.FromArgb("#E0E0E0"),
                            Padding = 15,
                            Content = new Label { Text = "Save\nLogo", FontSize = 20, HorizontalOptions = LayoutOptions.Center }
                        },
                        new Label { Text = "Save", FontSize = 40, VerticalOptions = LayoutOptions.Center }
                    }
                },
                new HorizontalStackLayout
                {
                    Spacing = 10,
                    Children =
                    {
                        new Label { Text = "Amount:", FontSize = 30, VerticalOptions = LayoutOptions.Center },
                        amountEntry
                    }
                },
                new HorizontalStackLayout
                {
                    Spacing = 10,
                    Children =
                    {
                        new Label { Text = "Remark:", FontSize = 30, VerticalOptions = LayoutOptions.Center },
                        remarkEntry
                    }
                },
                new HorizontalStackLayout
                {
                    Spacing = 40,
                    HorizontalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Button { Text = "Cancel", FontSize = 24, BackgroundColor = Color.FromArgb("#E0E0E0"), Padding = 10 },
                        new Button { Text = "Submit", FontSize = 24, BackgroundColor = Color.FromArgb("#E0E0E0"), Padding = 10 }
                    }
                }
            }
        };

        // 弹窗事件绑定
        var buttonRow = (HorizontalStackLayout)savePopup.Children[3];
        ((Button)buttonRow.Children[0]).Clicked += async (s, args) =>
        {
            await Navigation.PopModalAsync();
        };
        ((Button)buttonRow.Children[1]).Clicked += async (s, args) =>
        {
            if (decimal.TryParse(amountEntry.Text, out var amount) && amount > 0)
            {
                _totalAmount += amount;
                UpdateTotalAmountDisplay();
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("错误", "请输入有效的金额", "确定");
            }
        };

        await Navigation.PushModalAsync(new ContentPage
        {
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.5),
            Content = new Border
            {
                // 适配 .NET 10.0 圆角新语法
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(0) },
                Padding = 0,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Content = savePopup
            }
        });
    }

    // 点击 Cost 弹出弹窗
    private async void OnCostClicked(object sender, EventArgs e)
    {
        var amountEntry = new Entry { Placeholder = "input", Keyboard = Keyboard.Numeric };
        var remarkEntry = new Entry { Placeholder = "input" };

        var costPopup = new VerticalStackLayout
        {
            BackgroundColor = Color.FromArgb("#90EE90"),
            Padding = 20,
            Spacing = 15,
            Children =
            {
                new HorizontalStackLayout
                {
                    Spacing = 15,
                    Children =
                    {
                        new Border
                        {
                            // 适配 .NET 10.0 圆角新语法
                            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(50) },
                            BackgroundColor = Color.FromArgb("#E0E0E0"),
                            Padding = 15,
                            Content = new Label { Text = "Cost\nLogo", FontSize = 20, HorizontalOptions = LayoutOptions.Center }
                        },
                        new Label { Text = "Cost", FontSize = 40, VerticalOptions = LayoutOptions.Center }
                    }
                },
                new HorizontalStackLayout
                {
                    Spacing = 10,
                    Children =
                    {
                        new Label { Text = "Amount:", FontSize = 30, VerticalOptions = LayoutOptions.Center },
                        amountEntry
                    }
                },
                new HorizontalStackLayout
                {
                    Spacing = 10,
                    Children =
                    {
                        new Label { Text = "Remark:", FontSize = 30, VerticalOptions = LayoutOptions.Center },
                        remarkEntry
                    }
                },
                new HorizontalStackLayout
                {
                    Spacing = 40,
                    HorizontalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Button { Text = "Cancel", FontSize = 24, BackgroundColor = Color.FromArgb("#E0E0E0"), Padding = 10 },
                        new Button { Text = "Submit", FontSize = 24, BackgroundColor = Color.FromArgb("#E0E0E0"), Padding = 10 }
                    }
                }
            }
        };

        // 弹窗事件绑定
        var buttonRow = (HorizontalStackLayout)costPopup.Children[3];
        ((Button)buttonRow.Children[0]).Clicked += async (s, args) =>
        {
            await Navigation.PopModalAsync();
        };
        ((Button)buttonRow.Children[1]).Clicked += async (s, args) =>
        {
            if (decimal.TryParse(amountEntry.Text, out var amount) && amount > 0 && _totalAmount >= amount)
            {
                _totalAmount -= amount;
                UpdateTotalAmountDisplay();
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("错误", "请输入有效的金额（余额不足）", "确定");
            }
        };

        await Navigation.PushModalAsync(new ContentPage
        {
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.5),
            Content = new Border
            {
                // 适配 .NET 10.0 圆角新语法
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(0) },
                Padding = 0,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Stroke = Colors.Blue,
                StrokeThickness = 3,
                Content = costPopup
            }
        });
    }
}
