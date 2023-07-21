using ComputerStoreApp.Classes;
using ComputerStoreApp.Databases;
using ComputerStoreApp.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Логика взаимодействия для AddAccessoryPage.xaml
    /// </summary>
    public partial class AddAccessoryPage : Page
    {
        private readonly static OpenFileDialog _openFileDialog = new OpenFileDialog()
        {
            Multiselect = true,
            Filter = "Images (JPG;PNG;WEBP)| *.JPG;*.PNG;*.WEBP;*.jpg;*.png;*.webp"
        };

        public ObservableCollection<Photo> PhotoList { get; set; } = new ObservableCollection<Photo>();
        public ObservableCollection<BaseProperty> PropertyList { get; set; } = new ObservableCollection<BaseProperty>();

        private Accessory _currentAccessory;

        private List<Property> _unavailablePropertyList = new List<Property>();

        public Category _selectedCategory;

        public AddAccessoryPage(Accessory currentAccessory, Category selectedCategory)
        {
            InitializeComponent();

            _selectedCategory = selectedCategory;

            TypeComboBox.SelectedIndex = 0;
            PropertyComboBox.SelectedIndex = 0;

            if (currentAccessory != null)
            {
                IDTextBox.Text = currentAccessory.ID.ToString();
                _currentAccessory = currentAccessory;
                foreach (Photo photo in _currentAccessory.Photo)
                {
                    PhotoList.Add(new Photo
                    {
                        Photo1 = photo.Photo1
                    });
                }
                foreach(var property in _currentAccessory.NumberAccessoryProperty)
                {
                    NumberProperty numberProperty = new NumberProperty
                    {
                        Property = property.Property,
                        Number = property.Number,
                    };
                    PropertyList.Add(numberProperty);
                }
                foreach (var property in _currentAccessory.LinePropertyAccessory)
                {
                    LineProperty lineProperty = new LineProperty
                    {
                        Property = property.Property,
                        Line = property.Line,
                    };
                    PropertyList.Add(lineProperty);
                }
                foreach (var property in _currentAccessory.TypeAccessoryProperty)
                {
                    TypeProperty typeProperty = new TypeProperty
                    {
                        Property = property.Property,
                        Type = property.Property.Type.FirstOrDefault(x => x.ID.Equals(property.Type)),
                    };
                    PropertyList.Add(typeProperty);
                }
            }
            else
            {
                //var A = ComputerStoreDatabase.GetContext().Accessory.Last();
                //IDTextBox.Text = (A.ID + 1).ToString();
                _currentAccessory = new Accessory();
                _currentAccessory.Category = _selectedCategory;
            }
            AccessoryStackPanel.DataContext = _currentAccessory;
            PhotoListBox.ItemsSource = PhotoList;
            PropertyListBox.ItemsSource = PropertyList;

            CategoryTextBlock.Text = "Новый товар из категории " + _selectedCategory.Name;
            List<Firm> firmList = ComputerStoreDatabase.GetContext().Firm.ToList();
            firmList.Insert(0, new Firm
            {
                Name = "Выберите производителя"
            });
            FirmComboBox.ItemsSource = firmList.ToList();
            FirmComboBox.DisplayMemberPath = "Name";
            FirmComboBox.SelectedIndex = 0;
        }

        private void ClearPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            PhotoList.Clear();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 ||
                 e.Key == Key.Back || e.Key == Key.Right || e.Key == Key.Left)
            {
                e.Handled = false;
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void AddAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            #region Проверка полей
            StringBuilder errorStringBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(NameTextBox.Text))
            {
                errorStringBuilder.AppendLine("Введите наименование");
            }

            if (FirmComboBox.SelectedIndex < 1)
            {
                errorStringBuilder.AppendLine("Выберите фирму производителя");
            }

            if (string.IsNullOrEmpty(CostTextBox.Text))
            {
                errorStringBuilder.AppendLine("Введите стоимость");
            }
            else if (Convert.ToDecimal(CostTextBox.Text) < 1)
            {
                errorStringBuilder.AppendLine("Стоимость не может быть меньше одного рубля");
            }

            if (string.IsNullOrEmpty(QuantityTextBox.Text))
            {
                errorStringBuilder.AppendLine("Введите количество");
            }

            if (PhotoList.Count < 1)
            {
                errorStringBuilder.AppendLine("Добавьте хотя бы одно фото");
            }

            if (PropertyList.Count < 1 && MessageBox.Show("Вы уверены, что хотите продолжить?", "У товара отсутствуют свойства", MessageBoxButton.OKCancel)== MessageBoxResult.Cancel)
            {
                return;
            }

            if (errorStringBuilder.Length > 0)
            {
                MessageBox.Show(errorStringBuilder.ToString());
                return;
            }
            #endregion

            List<TypeAccessoryProperty> typePropertyList = new List<TypeAccessoryProperty>();
            List<NumberAccessoryProperty> numberPropertyList = new List<NumberAccessoryProperty>();
            List<LinePropertyAccessory> linePropertyList = new List<LinePropertyAccessory>();

            foreach (var property in PropertyList)
            {
                if (property as TypeProperty != null)
                {
                    typePropertyList.Add(new TypeAccessoryProperty
                    {
                        PropertyID = property.Property.ID,
                        Type = (property as TypeProperty).Type.ID,
                    });
                }
                else if (property as NumberProperty != null)
                {
                    numberPropertyList.Add(new NumberAccessoryProperty
                    {
                        PropertyID = property.Property.ID,
                        Number = (property as NumberProperty).Number,
                    });
                }
                else if (property as LineProperty != null)
                {
                    linePropertyList.Add(new LinePropertyAccessory
                    {
                        PropertyID = property.Property.ID,
                        Line = (property as LineProperty).Line,
                    });
                }
            }

            _currentAccessory.LinePropertyAccessory = linePropertyList;
            _currentAccessory.NumberAccessoryProperty = numberPropertyList;
            _currentAccessory.TypeAccessoryProperty = typePropertyList;
            ComputerStoreDatabase.GetContext().Photo.RemoveRange(_currentAccessory.Photo);
            _currentAccessory.Photo = PhotoList;
            ComputerStoreDatabase.GetContext().Accessory.AddOrUpdate(_currentAccessory);
            try
            {
                ComputerStoreDatabase.GetContext().SaveChanges();
                MessageBox.Show("Товар успешно добавлен");
                NavigationService.GoBack();
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

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_openFileDialog.ShowDialog() == true)
            {
                foreach(var photo in _openFileDialog.FileNames)
                {
                    PhotoList.Add(new Photo
                    {
                    Photo1 = File.ReadAllBytes(photo)
                    });
                }
                //PhotoImageBrush.ImageSource = new BitmapImage(new Uri(_openFileDialog.FileName, UriKind.Absolute));
                //_photoAccessory = File.ReadAllBytes(_openFileDialog.FileName);
            }
        }

        private void DeletePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Photo selectedPhoto = button.DataContext as Photo;
            PhotoList.Remove(selectedPhoto);
        }

        private void AddFirmButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeComboBox.SelectedIndex > 0)
            {
                List<Property> propertyList = _selectedCategory.Property.ToList();
                propertyList = propertyList.Except(_unavailablePropertyList).ToList();
                propertyList = propertyList.Where(x => x.Form == (Convert.ToByte(TypeComboBox.SelectedIndex))).ToList();
                propertyList.Insert(0, new Property
                {
                    Name = "Выберите свойство"
                });
                PropertyComboBox.ItemsSource = propertyList.ToList();
                PropertyComboBox.DisplayMemberPath = "Name";
                PropertyComboBox.IsEnabled = true;
            }
            else
            {
                PropertyComboBox.IsEnabled = false;
            }
            PropertyComboBox.SelectedIndex = 0;
        }

        private void UpdatePropertyList()
        {

        }

        private void AddPropertyButton_Click(object sender, RoutedEventArgs e)
        {
            PropertyWindow window = new PropertyWindow(null);
            window.ShowDialog();
        }

        private void AddAccessoryPropertyButton_Click(object sender, RoutedEventArgs e)
        {
            if (TypeComboBox.SelectedIndex < 1)
            {
                MessageBox.Show("Выберите тип свойства");
                return;
            }
            else if (PropertyComboBox.SelectedIndex < 1)
            {
                MessageBox.Show("Выберите свойство");
                return;
            }
            else if (TypeComboBox.SelectedIndex == 1) 
            {
                NumberProperty numberProperty = new NumberProperty
                {
                    Property = PropertyComboBox.SelectedItem as Property,
                };
                PropertyList.Add(numberProperty);
            }
            else if (TypeComboBox.SelectedIndex == 2) 
            {
                LineProperty lineProperty = new LineProperty
                {
                    Property = PropertyComboBox.SelectedItem as Property,
                };
                PropertyList.Add(lineProperty);
            }
            else if (TypeComboBox.SelectedIndex == 3)
            {
                TypeProperty typeProperty = new TypeProperty
                {
                    Property = PropertyComboBox.SelectedItem as Property,
                };
                PropertyList.Add(typeProperty);
            }

            _unavailablePropertyList.Add(PropertyComboBox.SelectedItem as Property);

            TypeComboBox.SelectedIndex = 0;
            ChangePropertyButton.IsEnabled = false;
        }

        private void RemovePropertyButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            BaseProperty property = (BaseProperty)button.DataContext;
            PropertyList.Remove(property);
            _unavailablePropertyList.Remove(property.Property);
        }

        private void ChangePropertyButton_Click(object sender, RoutedEventArgs e)
        {
            Property selectedProperty = PropertyComboBox.SelectedItem as Property;
            if (selectedProperty != null)
            {
                PropertyWindow window = new PropertyWindow(selectedProperty);
                window.ShowDialog();
            }
        }

        private void PropertyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedIndex < 1)
            {
                ChangePropertyButton.IsEnabled = false;
            }
            else
            {
                ChangePropertyButton.IsEnabled = true;
            }
        }
    }
}
