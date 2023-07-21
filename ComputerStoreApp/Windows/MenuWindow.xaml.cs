using ComputerStoreApp.Databases;
using ComputerStoreApp.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ComputerStoreApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        private Account _selectedAccount;
        public MenuWindow(Account selectedAccount)
        {
            InitializeComponent();
            _selectedAccount = selectedAccount;
            DataContext = _selectedAccount;
            if (selectedAccount.Access == 1)
            {
                AccessTextBlock.Text = "System admin";
            }
            else if (selectedAccount.Access == 2)
            {
                AccessTextBlock.Text = "Worker";
            }
            MenuFrame.Navigate(new MenuPage());
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimizedButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void StateButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                FullScreenIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.FullscreenExit;
            }
            else
            {
                WindowState = WindowState.Normal;
                FullScreenIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Fullscreen;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            MenuFrame.Navigate(new MenuPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow openWindow = new OpenWindow();
            openWindow.Show();
            Close();
        }
    }
}
