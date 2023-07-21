using ComputerStoreApp.Databases;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
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
    /// Логика взаимодействия для AddCategoryPage.xaml
    /// </summary>
    public partial class AddCategoryPage : Page
    {
        private readonly static OpenFileDialog _openFileDialog = new OpenFileDialog()
        {
            Multiselect = false,
            Filter = "Images (JPG;PNG;)| *.JPG;*.PNG;*.jpg;*.png"
        };
        private byte[] _photoCategory;
        private int _idCategory;

        public AddCategoryPage(Category currentCategory)
        {
            InitializeComponent();
            List<Group> groupList = ComputerStoreDatabase.GetContext().Group.ToList();
            groupList.Insert(0, new Group
            {
                Name = "Выберите группу"
            });
            GroupComboBox.ItemsSource = groupList.ToList();
            GroupComboBox.DisplayMemberPath = "Name";
            GroupComboBox.SelectedIndex = 0;
            if (currentCategory != null)
            {
                _idCategory = currentCategory.ID;
                NameTextBox.Text = currentCategory.Name;
                GroupComboBox.SelectedItem = currentCategory.Group;
                IsComputerCheckBox.IsChecked = currentCategory.IsComputer;
                IsRequiredCheckBox.IsChecked = currentCategory.IsRequired;
                _photoCategory = currentCategory.Photo;
                HEXTextBox.Text = currentCategory.ColorBackground;
                BrushConverter brushConverter = new BrushConverter();
                BackgroundBorder.Background = BackgroundColorPicker.Background = (Brush)brushConverter.ConvertFrom(currentCategory.ColorBackground);
                PhotoImageBrush.ImageSource = LoadImage(currentCategory.Photo);
            }
        }
        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            #region Проверка полей
            StringBuilder errorStringBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(NameTextBox.Text))
            {
                errorStringBuilder.AppendLine("Введите наименование");
            }

            if (GroupComboBox.SelectedIndex < 1)
            {
                errorStringBuilder.AppendLine("Выберите группу");
            }

            if (string.IsNullOrEmpty(HEXTextBox.Text))
            {
                errorStringBuilder.AppendLine("Выберите цвет");
            }

            if (_photoCategory == null)
            {
                errorStringBuilder.AppendLine("Добавьте фото");
            }

            if (errorStringBuilder.Length > 0)
            {
                MessageBox.Show(errorStringBuilder.ToString());
                return;
            }
            #endregion
            if (_idCategory > 0)
            {
                ComputerStoreDatabase.GetContext().Category.AddOrUpdate(new Category
                {
                    ID = _idCategory,
                    Name = NameTextBox.Text,
                    GroupID = (GroupComboBox.SelectedItem as Group).ID,
                    IsComputer = (bool)IsComputerCheckBox.IsChecked,
                    IsRequired = (bool)IsRequiredCheckBox.IsChecked,
                    Photo = _photoCategory,
                    ColorBackground = HEXTextBox.Text,
                });
            }
            else
            {
                ComputerStoreDatabase.GetContext().Category.Add(new Category
                {
                    Name = NameTextBox.Text,
                    GroupID = (GroupComboBox.SelectedItem as Group).ID,
                    IsComputer = (bool)IsComputerCheckBox.IsChecked,
                    IsRequired = (bool)IsRequiredCheckBox.IsChecked,
                    Photo = _photoCategory,
                    ColorBackground = HEXTextBox.Text,
                });
            }
            try
            {
                ComputerStoreDatabase.GetContext().SaveChanges();
                MessageBox.Show("Категория успешно добавлена");
                NavigationService.GoBack();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message); 
            }

        }

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_openFileDialog.ShowDialog() == true)
            {
                PhotoImageBrush.ImageSource = new BitmapImage(new Uri(_openFileDialog.FileName, UriKind.Absolute));
                _photoCategory = File.ReadAllBytes(_openFileDialog.FileName);
            }
        }

        private void ClearPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            _photoCategory = null;
            PhotoImageBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/ImageStub.png", UriKind.Absolute));
        }

        private void BackgroundColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            HEXTextBox.Text = BackgroundColorPicker.SelectedColor.ToString();

            BrushConverter brushConverter = new BrushConverter();

            BackgroundBorder.Background = BackgroundColorPicker.Background = (Brush)brushConverter.ConvertFrom(BackgroundColorPicker.SelectedColor.ToString());
        }
    }
}
