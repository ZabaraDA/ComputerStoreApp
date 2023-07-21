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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ComputerStoreApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для CategoryPage.xaml
    /// </summary>
    public partial class CategoryPage : Page
    {
        public CategoryPage()
        {
            InitializeComponent();
            FilterCategory();
        }

        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddCategoryPage(null));
        }

        private void FilterCategory()
        {
            List<Category> categoryList = ComputerStoreDatabase.GetContext().Category.ToList();

            categoryList = categoryList.Where(category => category.Name.ToLower().Contains(NameSearchTextBox.Text.ToLower())).ToList();

            CategoryListView.ItemsSource = categoryList.ToList();
        }

        private void ChangeCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Category selectedCategory = (Category)button.DataContext;
            if (selectedCategory != null)
            {
                NavigationService.Navigate(new AddCategoryPage(selectedCategory));
            }
        }

        private void NameSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterCategory();
        }
    }
}
