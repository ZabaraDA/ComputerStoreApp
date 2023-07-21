using ComputerStoreApp.Databases;
using ComputerStoreApp.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ComputerStoreApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class OpenWindow : Window
    {
        public OpenWindow()
        {
            InitializeComponent();
            LoginTextBox.Text = "ad";
            PasswordBox.Password = "ad";
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LoginTextBox.Text) || string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("Введите логин и/или пароль!");
                return;
            }

            Account selectedAccount = ComputerStoreDatabase.GetContext().Account.FirstOrDefault(account => account.Login.Equals(LoginTextBox.Text) && account.Password.Equals(PasswordBox.Password));

            if (selectedAccount != null)
            {
                MenuWindow window = new MenuWindow(selectedAccount);
                window.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Логин и/или пароль были введены неверно!");
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
