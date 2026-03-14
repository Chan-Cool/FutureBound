using Microsoft.Maui.Controls;
using FutureBound.Page;

namespace FutureBound
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // 设置启动页
            MainPage = new NavigationPage(new SplashPage());
        }
    }
}