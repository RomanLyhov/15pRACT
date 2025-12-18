using Pract15.Models;
using Pract15.Service;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Pract15
{
    public partial class AddCategor : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly CategoryService _service = new CategoryService();
        private Category _currentCategory;

        public Category CurrentCategory
        {
            get => _currentCategory;
            set
            {
                _currentCategory = value;
                OnPropertyChanged(nameof(CurrentCategory));
            }
        }

        private bool _isEditMode = false;
        public ICollectionView CategoriesView { get; set; }
        public string SearchQuery { get; set; } = string.Empty;

        public AddCategor()
        {
            InitializeComponent();

            CategoriesView = CollectionViewSource.GetDefaultView(_service.Categories);
            CategoriesView.Filter = FilterCategories;
            CurrentCategory = new Category();
            DataContext = this;

        }

        public bool FilterCategories(object obj)
        {
            if (obj is not Category)
                return false;

            var category = (Category)obj;
            if (!string.IsNullOrEmpty(SearchQuery) &&
                !category.Name.Contains(SearchQuery, StringComparison.CurrentCultureIgnoreCase))
                return false;

            return true;
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CategoriesView.Refresh();

            btnSave.IsEnabled = !string.IsNullOrWhiteSpace(txtCategoryName.Text);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string categoryName = txtCategoryName.Text?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(categoryName))
                {
                    MessageBox.Show("Название категории не может быть пустым!", "Ошибка",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                    txtCategoryName.Focus();
                    return;
                }

                if (_isEditMode)
                {
                    _currentCategory.Name = categoryName;
                    _service.Update(_currentCategory);

                    MessageBox.Show("Категория успешно обновлена!", "Успех",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var newCategory = new Category { Name = categoryName };
                    _service.Add(newCategory);

                    MessageBox.Show("Категория успешно добавлена!", "Успех",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }

                CategoriesView.Refresh();

                ResetForm();
                txtCategoryName.Focus();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка валидации",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Category category)
            {
                _isEditMode = true;
                _currentCategory = category;
                txtCategoryName.Text = category.Name;
                btnSave.Content = "Обновить";
                btnSave.IsEnabled = true;
                txtCategoryName.Focus();
                txtCategoryName.SelectAll();
                MessageBox.Show($"Редактирование категории: {category.Name}\nВнесите изменения и нажмите 'Обновить'",
                               "Режим редактирования",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Category category)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить категорию '{category.Name}'?",
                                           "Подтверждение удаления",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (_isEditMode && _currentCategory.Id == category.Id)
                        {
                            ResetForm();
                        }

                        _service.Remove(category);
                        CategoriesView.Refresh();

                        MessageBox.Show("Категория успешно удалена!", "Успех",
                                       MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                                       MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void ResetForm()
        {
            _isEditMode = false;
            _currentCategory = new Category();
            txtCategoryName.Text = string.Empty;
            CurrentCategory = new Category();
            btnSave.Content = "Сохранить";
            btnSave.IsEnabled = false;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}