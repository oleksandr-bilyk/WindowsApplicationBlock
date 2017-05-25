﻿using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.MainPage;
using Tampleworks.WindowsApplicationBlock.WindowsUniversalView;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Tampleworks.WindowsApplicationBlock.Demo.UwpApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public MainPageViewModel ViewModel { get; private set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var prameter = (PageNavitedToParameters)e.Parameter;
            ViewModel = (MainPageViewModel)prameter.ViewModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        public static Brush IsFavoriteToBrush(bool isFavorite)
            => isFavorite ? new SolidColorBrush(Colors.Orange) : null;
    }
}