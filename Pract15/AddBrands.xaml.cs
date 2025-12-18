using Pract15.Models;
using Pract15.Service;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Pract15
{
    public partial class AddBrands : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly BrandsService _service = new BrandsService();
        private bool _isEditMode = false;

        private Brand _currentBrand = new Brand();
        public Brand CurrentBrand
        {
            get => _currentBrand;
            set
            {
                _currentBrand = value;
                OnPropertyChanged(nameof(CurrentBrand));
            }
        }

        public ObservableCollection<Brand> Brands { get; set; }

        public AddBrands()
        {
            InitializeComponent();
            LoadBrands();
            DataContext = this;
        }

        private void LoadBrands()
        {
            Brands = new ObservableCollection<Brand>(_service.Brands);
            OnPropertyChanged(nameof(Brands));
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CurrentBrand.Name))
                {
                    MessageBox.Show("Название бренда не может быть пустым!", "Ошибка",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                    txtBrandName.Focus();
                    return;
                }

                if (_isEditMode)
                {
                    _service.Update(CurrentBrand);
                    MessageBox.Show("Бренд успешно обновлен!", "Успех",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var newBrand = new Brand
                    {
                        Name = CurrentBrand.Name.Trim()
                    };

                    _service.Add(newBrand);
                    MessageBox.Show("Бренд успешно добавлен!", "Успех",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
                LoadBrands();
                ResetForm();
                txtBrandName.Focus();
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
            if (sender is Button button && button.Tag is Brand brand)
            {
                _isEditMode = true;
                CurrentBrand = new Brand
                {
                    Id = brand.Id,
                    Name = brand.Name
                };
                btnSave.Content = "Обновить";
                txtBrandName.Focus();
                txtBrandName.SelectAll();

                MessageBox.Show($"Редактирование бренда: {brand.Name}\nВнесите изменения и нажмите 'Обновить'",
                               "Режим редактирования",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Brand brand)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить бренд '{brand.Name}'?",
                                           "Подтверждение удаления",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (_isEditMode && CurrentBrand.Id == brand.Id)
                        {
                            ResetForm();
                        }

                        _service.Remove(brand);
                        LoadBrands();

                        MessageBox.Show("Бренд успешно удален!", "Успех",
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
            CurrentBrand = new Brand();
            btnSave.Content = "Сохранить";
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}