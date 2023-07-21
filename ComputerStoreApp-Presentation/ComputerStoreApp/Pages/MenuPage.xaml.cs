using ComputerStoreApp.Databases;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ComputerStoreApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для MenuPagePage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        private readonly ThicknessAnimation _doubleAnimation = new ThicknessAnimation();
        public MenuPage()
        {
            InitializeComponent();
            FilterCategory();
            
        }
        private void FilterCategory()
        {
            List<Category> categoryList = ComputerStoreDatabase.GetContext().Category.ToList();
            MenuListView.ItemsSource = categoryList.ToList();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _doubleAnimation.From = new Thickness(0);
            _doubleAnimation.To = new Thickness(-300,0,0,0);
            _doubleAnimation.Duration = TimeSpan.FromSeconds(0.5);
            _doubleAnimation.EasingFunction = new QuadraticEase();
            MenuStackPanel.BeginAnimation(MarginProperty, _doubleAnimation);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _doubleAnimation.From = new Thickness(-300,0,0,0);
            _doubleAnimation.To = new Thickness(0, 0, 0, 0);
            _doubleAnimation.Duration = TimeSpan.FromSeconds(0.5);
            _doubleAnimation.EasingFunction = new QuadraticEase();
            MenuStackPanel.BeginAnimation(MarginProperty, _doubleAnimation);
        }

        private void HandbookButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ComputerConfuguratorPage());
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CategoryPage());
        }

        private void ViewAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Category selectedCategory = (Category)button.DataContext;
            if (selectedCategory != null)
            {
                NavigationService.Navigate(new AccessoryPage(selectedCategory));
            }
            //MessageBox.Show(selectedCategory.Accessory.Count.ToString());
        }
    }
}
