using Pract15.Models;
using Pract15.Service;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Pract15
{
    public partial class AddProd : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Product _product = new Product();
        public Product CurrentProduct
        {
            get => _product;
            set
            {
                _product = value;
                OnPropertyChanged(nameof(CurrentProduct));
            }
        }

        private readonly ProductService _service = new ProductService();
        private readonly CategoryService _categoryService = new CategoryService();
        private readonly BrandsService _brandService = new BrandsService();
        private readonly TagService _tagService = new TagService();

        private bool _isEditMode = false;

        public ObservableCollection<Category> Categories => _categoryService.Categories;
        public ObservableCollection<Brand> Brands => _brandService.Brands;

        private ObservableCollection<TagCheckBox> _allTags = new();
        public ObservableCollection<TagCheckBox> AllTags
        {
            get => _allTags;
            set
            {
                _allTags = value;
                OnPropertyChanged(nameof(AllTags));
            }
        }

        public AddProd()
        {
            InitializeComponent();
            LoadTags();
            DataContext = this;
        }

        public AddProd(Product product) : this()
        {
            _isEditMode = true;

            CurrentProduct.Id = product.Id;
            CurrentProduct.Name = product.Name;
            CurrentProduct.Description = product.Description;
            CurrentProduct.Price = product.Price;
            CurrentProduct.Stock = product.Stock;
            CurrentProduct.Rating = product.Rating;
            CurrentProduct.CategoryId = product.CategoryId;
            CurrentProduct.BrandId = product.BrandId;
            CurrentProduct.CreatedAt = product.CreatedAt;
            CurrentProduct.Tags.Clear();
            foreach (var tag in product.Tags)
            {
                CurrentProduct.Tags.Add(tag);
            }

            foreach (var tagCheckBox in AllTags)
            {
                tagCheckBox.IsSelected = product.Tags.Any(t => t.Id == tagCheckBox.Tag.Id);
            }

            if (btnSave != null)
            {
                btnSave.Content = "Обновить";
            }
        }

        private void LoadTags()
        {
            var tags = new ObservableCollection<TagCheckBox>();
            foreach (var tag in _tagService.Tags)
            {
                tags.Add(new TagCheckBox { Tag = tag, IsSelected = false });
            }
            AllTags = tags;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(txtName) ||
        Validation.GetHasError(txtPrice) ||
        Validation.GetHasError(txtStock) ||
        Validation.GetHasError(txtRating))
            {
                MessageBox.Show("Исправьте ошибки в форме", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                CurrentProduct.Tags.Clear();
                foreach (var tagCheckBox in AllTags.Where(t => t.IsSelected))
                {
                    CurrentProduct.Tags.Add(tagCheckBox.Tag);
                }

                if (string.IsNullOrWhiteSpace(CurrentProduct.Name))
                {
                    MessageBox.Show("Название товара не может быть пустым!", "Ошибка",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!CurrentProduct.CategoryId.HasValue || CurrentProduct.CategoryId <= 0)
                {
                    MessageBox.Show("Выберите категорию!", "Ошибка",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!CurrentProduct.BrandId.HasValue || CurrentProduct.BrandId <= 0)
                {
                    MessageBox.Show("Выберите бренд!", "Ошибка",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_isEditMode)
                {
                    _service.Update(CurrentProduct);
                    MessageBox.Show("Товар успешно обновлен!", "Успех",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _service.Add(CurrentProduct);
                    MessageBox.Show("Товар успешно добавлен!", "Успех",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }

                NavigationService.GoBack();
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Tag tag)
            {
                // Здесь должна быть логика редактирования тега
                // Например, открытие окна редактирования или заполнение формы
                MessageBox.Show($"Редактирование тега: {tag.Name}", "Редактирование",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Tag tag)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить тег \"{tag.Name}\"?",
                                           "Подтверждение удаления",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _tagService.Remove(tag);
                        LoadTags(); // Перезагружаем теги после удаления
                        MessageBox.Show("Тег успешно удален!", "Успех",
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

        public class TagCheckBox : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private Tag _tag;
            public Tag Tag
            {
                get => _tag;
                set
                {
                    _tag = value;
                    OnPropertyChanged(nameof(Tag));
                    OnPropertyChanged(nameof(Name));
                }
            }
            private bool _isSelected;
            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }

            public string Name => Tag?.Name ?? string.Empty;
        }
        public string RatingText
        {
            get => CurrentProduct.Rating?.ToString(CultureInfo.CurrentCulture) ?? string.Empty;
            set
            {
                if (decimal.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal r))
                {
                    CurrentProduct.Rating = r;
                }
                else
                {
                    CurrentProduct.Rating = null;
                }
                OnPropertyChanged(nameof(RatingText));
            }
        }
    }
}