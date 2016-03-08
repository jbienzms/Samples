using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ThemeSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {


        public MainPage()
        {
            ThemeManager.Initialize();
            this.InitializeComponent();
        }

        private void Reload()
        {
            ((Frame)Window.Current.Content).Navigate(typeof(MainPage));
        }

        private void UpdateColors(string name)
        {
            if (ThemeManager.UpdateColors(name))
            {
                Reload();
            }
        }

        private void UpdateShapes(string name)
        {
            if (ThemeManager.UpdateShapes(name))
            {
                Reload();
            }
        }

        private void RedButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateColors("Red");
        }

        private void GreenButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateColors("Green");
        }

        private void EllipseButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateShapes("Ellipse");
        }

        private void RectButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateShapes("Rect");
        }
    }
}
