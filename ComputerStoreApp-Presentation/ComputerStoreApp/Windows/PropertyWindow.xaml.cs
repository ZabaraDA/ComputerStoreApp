using ComputerStoreApp.Classes;
using ComputerStoreApp.Databases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
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

namespace ComputerStoreApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для PropertyWindow.xaml
    /// </summary>

    public partial class PropertyWindow : Window
    {

        private ObservableCollection<Databases.Type> _typeList = new ObservableCollection<Databases.Type>();

        private List<IsAvailableCategoryProperty> _categoryList;

        private List<Category> _expectCategoryList = new List<Category>();

        private Property _currentProperty;

        private bool _handleSelection = true;

        private int _categoryCount;

        public PropertyWindow(Property currentProperty)
        {
            InitializeComponent();
            List<Unit> unitList = ComputerStoreDatabase.GetContext().Unit.ToList();
            unitList.Insert(0, new Unit
            {
                Name = "Выберите единицу измерения"
            });
            UnitComboBox.ItemsSource = unitList.ToList();
            UnitComboBox.DisplayMemberPath = "Name";

            _categoryList = new List<IsAvailableCategoryProperty>();
            if (currentProperty != null)
            {
                IDTextBox.Text = currentProperty.ID.ToString();
                _currentProperty = currentProperty;
                //    _categoryList = currentProperty.Category.ToList();           
                foreach(var category in _currentProperty.Category) 
                {
                    _categoryList.Add(new IsAvailableCategoryProperty
                    {
                        Category = category,
                        IsAvailable = true
                    });
                    _expectCategoryList.Add(category);
                    _categoryCount++;
                }
            }
            else 
            {
                IDTextBox.Text = ComputerStoreDatabase.GetContext().Property.Count().ToString();
                _currentProperty = new Property();
                UnitComboBox.SelectedIndex = 0;
                FormComboBox.SelectedIndex = 0;
            }
            CategoryUpdate();
            DataContext = _currentProperty;

            TypeListBox.ItemsSource = _typeList;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MinimizedButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CategoryUpdate()
        {
            List<Category> categoryList = ComputerStoreDatabase.GetContext().Category.ToList();
            MaxCountTextBlock.Text = " из " + categoryList.Count.ToString();
            categoryList = categoryList.Except(_expectCategoryList).ToList();
            List<IsAvailableCategoryProperty> categoryList1 = new List<IsAvailableCategoryProperty>();
            foreach (var category in categoryList)
            {
                categoryList1.Add(new IsAvailableCategoryProperty 
                { 
                    Category = category, 
                    IsAvailable = false 
                });
            }
            _categoryList.AddRange(categoryList1);
            CategoryListBox.ItemsSource = _categoryList.ToList();
            CountTextBlock.Text = "Выбрано " + _categoryCount.ToString();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            IsAvailableCategoryProperty selectedCategory = checkBox.DataContext as IsAvailableCategoryProperty;
            _categoryList.Add(selectedCategory);
            _categoryCount++;
            CountTextBlock.Text = "Выбрано " + _categoryCount.ToString();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            IsAvailableCategoryProperty selectedCategory = checkBox.DataContext as IsAvailableCategoryProperty;
            _categoryList.Remove(selectedCategory);
            _categoryCount--;
            CountTextBlock.Text = "Выбрано " + _categoryCount.ToString();
        }

        private void SavePropertyButton_Click(object sender, RoutedEventArgs e)
        {
            #region Проверка полей
            StringBuilder errorStringBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(NameTextBox.Text))
            {
                errorStringBuilder.AppendLine("Введите наименование");
            }

            if (UnitComboBox.SelectedIndex < 1)
            {
                errorStringBuilder.AppendLine("Выберите единицу измерения");
            }

            if (_typeList.Count < 2 && FormComboBox.SelectedIndex == 3)
            {
                errorStringBuilder.AppendLine("У свойства должно быть хотя бы 2 типа");
            }

            if (string.IsNullOrEmpty(DescriptionTextBox.Text) && MessageBox.Show("Вы уверены, что хотите продолжить?", "У свойства отсутствует описание", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }

            List<Category> categoryList = new List<Category>();
            foreach (var category in _categoryList)
            {
                if (category.IsAvailable == true)
                {
                    categoryList.Add(category.Category);
                }
            }
            if (categoryList.Count < 1)
            {
                errorStringBuilder.AppendLine("Свойство должно быть применимо хотя бы к одной категории");
            }

            if (errorStringBuilder.Length > 0)
            {
                MessageBox.Show(errorStringBuilder.ToString());
                return;
            }
            #endregion

            _currentProperty.Category = categoryList;
            _currentProperty.Type = _typeList;
            ComputerStoreDatabase.GetContext().Property.AddOrUpdate(_currentProperty);
            try
            {
                ComputerStoreDatabase.GetContext().SaveChanges();
                MessageBox.Show("Свойство успешно добавлено");
                DialogResult = true;
                Close();
            }
            catch (DbEntityValidationException exception)
            {
                foreach (DbEntityValidationResult validationError in exception.EntityValidationErrors)
                {
                    MessageBox.Show("Object: " + validationError.Entry.Entity.ToString());
                    MessageBox.Show("");
                    foreach (DbValidationError err in validationError.ValidationErrors)
                    {
                        MessageBox.Show(err.ErrorMessage + "");
                    }
                }
            }
        }

        private void AddTypeButton_Click(object sender, RoutedEventArgs e)
        {
            _typeList.Add(new Databases.Type());
        }

        private void AddUnitButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteTypeButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Databases.Type selectedType = button.DataContext as Databases.Type;
            _typeList.Remove(selectedType);
        }

        private void FormComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_handleSelection == true)
            {
                if (_typeList.Count > 0 &&
                    MessageBox.Show("При изменении будут сброшены все типы свойства", "Вы уверены?", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    ComboBox comboBox = sender as ComboBox;
                    _handleSelection = false;
                    comboBox.SelectedItem = e.RemovedItems[0];
                    return;
                }
            }
            else
            {
                _handleSelection = true;
                return;
            }
            if (FormComboBox.SelectedIndex != 3)
            {
                _typeList.Clear();
                AddTypeButton.IsEnabled = false;
                UnitComboBox.SelectedIndex = 0;
                UnitComboBox.IsEnabled = true;
            }
            else
            {
                AddTypeButton.IsEnabled = true;
                UnitComboBox.SelectedIndex = 1;
                UnitComboBox.IsEnabled = false;
            }
        }
    }
}
