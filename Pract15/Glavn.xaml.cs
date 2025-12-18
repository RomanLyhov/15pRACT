using Microsoft.EntityFrameworkCore;
using Pract15.Models;
using Pract15.Service;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Pract15
{
    public partial class Glavn : Page, INotifyPropertyChanged
    {
        private bool isManager = false; 
        public Pract15Context db = DBService.Instance.Context;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Product> prods { get; set; } = new ObservableCollection<Product>();
        public ICollectionView prodsView { get; set; }

        private string _searchQuery = "";
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                prodsView?.Refresh();
            }
        }

        private string _filterPriceFrom = "";
        public string FilterPriceFrom
        {
            get => _filterPriceFrom;
            set
            {
                _filterPriceFrom = value;
                OnPropertyChanged();
                prodsView?.Refresh();
            }
        }

        private string _filterPriceTo = "";
        public string FilterPriceTo
        {
            get => _filterPriceTo;
            set
            {
                _filterPriceTo = value;
                OnPropertyChanged();
                prodsView?.Refresh();
            }
        }
        public Glavn(bool manager)
        {
            isManager = manager;
            InitializeComponent();
            DataContext = this;
            if (!isManager)
            {
                ButtonsPanel.Visibility = Visibility.Collapsed;
            }
            LoadCategoriesAndBrands();
            prodsView = CollectionViewSource.GetDefaultView(prods);
            prodsView.Filter = FilterProducts;
            prodsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            cmbSort.SelectedIndex = 0;
            Loaded += (s, e) => LoadList();
            if (isManager)
            {
                ProductsList.MouseDoubleClick += ProductsList_MouseDoubleClick;
            }
            cmbCategoryFilter.SelectionChanged += FilterComboBox_SelectionChanged;
            cmbBrandFilter.SelectionChanged += FilterComboBox_SelectionChanged;
        }

        private void LoadCategoriesAndBrands()
        {
            cmbCategoryFilter.Items.Clear();
            cmbCategoryFilter.Items.Add(new ComboBoxItem { Content = "Все категории", Tag = -1 });

            var categories = db.Categories.ToList();
            foreach (var cat in categories)
            {
                cmbCategoryFilter.Items.Add(new ComboBoxItem { Content = cat.Name, Tag = cat.Id });
            }

            cmbBrandFilter.Items.Clear();
            cmbBrandFilter.Items.Add(new ComboBoxItem { Content = "Все бренды", Tag = -1 });

            var brands = db.Brands.ToList();
            foreach (var brand in brands)
            {
                cmbBrandFilter.Items.Add(new ComboBoxItem { Content = brand.Name, Tag = brand.Id });
            }

            cmbCategoryFilter.SelectedIndex = 0;
            cmbBrandFilter.SelectedIndex = 0;
        }

        private void ProductsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isManager && ProductsList.SelectedItem is Product selectedProduct)
            {
                NavigationService.Navigate(new AddProd(selectedProduct));
            }
        }

        public void LoadList()
        {
            prods.Clear();
            var products = db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Tags)
                .ToList();

            foreach (var p in products)
            {
                prods.Add(p);
            }
            prodsView.Refresh();
        }

        public bool FilterProducts(object obj)
        {
            if (obj is not Product product) return false;
            if (!string.IsNullOrEmpty(SearchQuery) &&
                !product.Name.Contains(SearchQuery, StringComparison.CurrentCultureIgnoreCase))
                return false;
            if (!string.IsNullOrEmpty(FilterPriceFrom) &&
                decimal.TryParse(FilterPriceFrom, out decimal minPrice) &&
                product.Price < minPrice)
                return false;
            if (!string.IsNullOrEmpty(FilterPriceTo) &&
                decimal.TryParse(FilterPriceTo, out decimal maxPrice) &&
                product.Price > maxPrice)
                return false;
            if (cmbCategoryFilter.SelectedItem is ComboBoxItem catItem &&
                catItem.Tag != null)
            {
                if (catItem.Tag.ToString() != "-1")
                {
                    if (int.TryParse(catItem.Tag.ToString(), out int catId) &&
                        product.CategoryId != catId)
                        return false;
                }
            }
            if (cmbBrandFilter.SelectedItem is ComboBoxItem brandItem &&
                brandItem.Tag != null)
            {
                if (brandItem.Tag.ToString() != "-1")
                {
                    if (int.TryParse(brandItem.Tag.ToString(), out int brandId) &&
                        product.BrandId != brandId)
                        return false;
                }
            }

            return true;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SearchQuery = "";
            FilterPriceFrom = "";
            FilterPriceTo = "";
            cmbCategoryFilter.SelectedIndex = 0;
            cmbBrandFilter.SelectedIndex = 0;
            cmbSort.SelectedIndex = 0;
            prodsView.SortDescriptions.Clear();
            prodsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            prodsView.Refresh();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbSort.SelectedItem is ComboBoxItem selectedItem)
            {
                prodsView.SortDescriptions.Clear();

                switch (selectedItem.Tag?.ToString())
                {
                    case "NameAsc": prodsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending)); break;
                    case "NameDesc": prodsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending)); break;
                    case "PriceAsc": prodsView.SortDescriptions.Add(new SortDescription("Price", ListSortDirection.Ascending)); break;
                    case "PriceDesc": prodsView.SortDescriptions.Add(new SortDescription("Price", ListSortDirection.Descending)); break;
                    case "StockAsc": prodsView.SortDescriptions.Add(new SortDescription("Stock", ListSortDirection.Ascending)); break;
                    case "StockDesc": prodsView.SortDescriptions.Add(new SortDescription("Stock", ListSortDirection.Descending)); break;
                }

                prodsView.Refresh();
            }
        }
        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            prodsView.Refresh();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            prodsView.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddProd());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddCategor());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddTags());
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddBrands());
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        // В отдельном файле или в том же классе добавь конвертер:
        public class NullToBooleanConverter : IValueConverter
        {
            public static NullToBooleanConverter Instance { get; } = new NullToBooleanConverter();

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return value != null;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        // В классе Glavn исправь метод удаления:
        private void DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            if (!isManager) return;

            if (ProductsList.SelectedItem is Product selectedProduct)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить товар '{selectedProduct.Name}'?",
                                            "Подтверждение удаления",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Получаем актуальный продукт из базы данных
                        var productFromDb = db.Products
                            .Include(p => p.Tags)
                            .FirstOrDefault(p => p.Id == selectedProduct.Id);

                        if (productFromDb != null)
                        {
                            // Удаляем связи с тегами
                            productFromDb.Tags.Clear();

                            // Сохраняем изменения для удаления связей
                            db.SaveChanges();

                            // Удаляем сам продукт
                            db.Products.Remove(productFromDb);
                            db.SaveChanges();

                            // Удаляем из коллекции
                            prods.Remove(selectedProduct);

                            MessageBox.Show($"Товар '{selectedProduct.Name}' успешно удален",
                                          "Успех",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Information);

                            // Снимаем выделение
                            ProductsList.SelectedItem = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении товара: {ex.Message}",
                                      "Ошибка",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления",
                              "Внимание",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
            }
        }
    }
    
    
}