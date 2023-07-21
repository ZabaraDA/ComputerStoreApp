using ComputerStoreApp.Classes;
using ComputerStoreApp.Databases;
using ComputerStoreApp.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ComputerStoreApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для AccessoryPage.xaml
    /// </summary>
    public partial class AccessoryPage : Page
    {
        private ObservableCollection<BaseProperty> _propertyList = new ObservableCollection<BaseProperty>();

        private Category _selectedCategory;

        public AccessoryPage(Category selectedCategory)
        {
            InitializeComponent();
            _selectedCategory = selectedCategory;
            PropertyUpload();
            FilterAccessory();
        }

        private void PropertyUpload()
        {
            foreach (var property in _selectedCategory.Property)
            {
                if (property.Form == 3)
                {
                    List<IsAvailableTypeProperty> typeList = new List<IsAvailableTypeProperty>();
                    foreach (var type in property.Type)
                    {
                        typeList.Add(new IsAvailableTypeProperty
                        {
                            Type = type,
                            IsAvailable = false
                        });
                    }
                    _propertyList.Add(new FilterTypeProperty
                    {
                        Property = property,
                        Type = typeList
                    });
                }
            }
            PropertyListBox.ItemsSource = _propertyList;
        }

        async protected void StartFilter()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FilterAccessory();
                });
            });
        }
        private void FilterAccessory()
        {
            List<Accessory> accessoryList = _selectedCategory.Accessory.ToList();

            if (!string.IsNullOrEmpty(NameSearchTextBox.Text))
            {
                accessoryList = accessoryList.Where(x => x.Name.ToLower().Contains(NameSearchTextBox.Text.ToLower())).ToList();
            }
            List<Accessory> accessoriesList = new List<Accessory>();
            bool IsIntersect = false;
            foreach (var property in _propertyList)
            {
                if (property.Property.Form == 3)
                {
                    foreach (var type in (property as FilterTypeProperty).Type)
                    {
                        if (type.IsAvailable == true)
                        {
                            IsIntersect = true;
                            List<TypeAccessoryProperty> typeAccessoryPropertyList = ComputerStoreDatabase.GetContext().TypeAccessoryProperty.
                                Where(x => x.PropertyID.Equals(property.Property.ID) && x.Type.Equals(type.Type.ID)).ToList();
                            foreach (var typeAccessoryProperty in typeAccessoryPropertyList)
                            {
                                accessoriesList.Add(typeAccessoryProperty.Accessory);
                            }
                        }
                    }
                }
            }
            if (IsIntersect == true)
            {
                accessoryList = accessoryList.Intersect(accessoriesList).ToList();
            }

            #region CheckBoxFilter
            if (NameToggleButton.IsChecked != null)
            {
                if (NameToggleButton.IsChecked == true)
                {
                    accessoryList = accessoryList.OrderBy(x => x.Name).ToList();
                }
                else
                {
                    accessoryList = accessoryList.OrderByDescending(x => x.Name).ToList();
                }
            }

            if (CostToggleButton.IsChecked != null)
            {
                if (CostToggleButton.IsChecked == true)
                {
                    accessoryList = accessoryList.OrderBy(x => x.Cost).ToList();
                }
                else
                {
                    accessoryList = accessoryList.OrderByDescending(x => x.Cost).ToList();
                }
            }
            #endregion

            AccessoryListView.ItemsSource = accessoryList.ToList();
        }

        private void CostToggleButton_Click(object sender, RoutedEventArgs e)
        {
            NameToggleButton.IsChecked = null;
            StartFilter();
        }

        private void NameToggleButton_Click(object sender, RoutedEventArgs e)
        {
            CostToggleButton.IsChecked = null;
            StartFilter();
        }

        private void InfoAccessoryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Accessory selectedAccessory = button.DataContext as Accessory;
            NavigationService.Navigate(new AddAccessoryPage(selectedAccessory, _selectedCategory));
        }

        private void DectivationAccessoryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddAccessoryPage(null, _selectedCategory));
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartFilter();
        }

        private void SelectedTypeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            FilterTypeProperty property = checkBox.DataContext as FilterTypeProperty;
            foreach (var type in property.Type)
            {
                type.IsAvailable = true;
            }
        }

        private void SelectedTypeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            FilterTypeProperty property = checkBox.DataContext as FilterTypeProperty;
            foreach (var type in property.Type)
            {
                type.IsAvailable = false;
            }
        }

        private void ClearTypeButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            FilterTypeProperty property = button.DataContext as FilterTypeProperty;
            foreach (var type in property.Type)
            {
                type.IsAvailable = false;
            }
        }

        private void FilterAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            FilterAccessory();

        }

        //private void Popup_Opened(object sender, EventArgs e)
        //{
        //    DispatcherTimer time = new DispatcherTimer();
        //    time.Interval = TimeSpan.FromSeconds(5);
        //    time.Start();
        //    time.Tick += delegate
        //    {
        //        (sender as Popup).IsOpen = false;
        //    };
        //}
    }
}
