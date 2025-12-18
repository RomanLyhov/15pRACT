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
    public partial class AddTags : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly TagService _service = new TagService();
        private Tag _currentTag;

        public Tag CurrentTag
        {
            get => _currentTag;
            set
            {
                _currentTag = value;
                OnPropertyChanged(nameof(CurrentTag));
            }
        }

        private bool _isEditMode = false;

        public ICollectionView TagsView { get; set; }
        public string SearchQuery { get; set; } = string.Empty;

        public AddTags()
        {
            InitializeComponent();
            CurrentTag = new Tag();
            TagsView = CollectionViewSource.GetDefaultView(_service.Tags);
            TagsView.Filter = FilterTags;

            DataContext = this;
            TagsListView.ItemsSource = TagsView;
        }

        private bool FilterTags(object obj)
        {
            if (obj is not Tag tag) return false;

            if (string.IsNullOrEmpty(SearchQuery)) return true;

            return tag.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TagsView.Refresh();
            btnSave.IsEnabled = !string.IsNullOrWhiteSpace(txtTagName.Text);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tagName = txtTagName.Text?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(tagName))
                {
                    MessageBox.Show("Название тега не может быть пустым!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    txtTagName.Focus();
                    return;
                }

                if (_isEditMode)
                {
                    _currentTag.Name = tagName;
                    _service.Update(_currentTag);
                    MessageBox.Show("Тег успешно обновлен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var newTag = new Tag { Name = tagName };
                    _service.Add(newTag);
                    MessageBox.Show("Тег успешно добавлен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                TagsView.Refresh();
                ResetForm();
                txtTagName.Focus();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Tag tag)
            {
                _isEditMode = true;
                _currentTag = tag;

                txtTagName.Text = tag.Name;
                btnSave.Content = "Обновить";
                btnSave.IsEnabled = true;

                txtTagName.Focus();
                txtTagName.SelectAll();

                MessageBox.Show($"Редактирование тега: {tag.Name}",
                    "Режим редактирования", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Tag tag)
            {
                var result = MessageBox.Show($"Удалить тег '{tag.Name}'?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (_isEditMode && _currentTag.Id == tag.Id)
                            ResetForm();

                        _service.Remove(tag);
                        TagsView.Refresh();

                        MessageBox.Show("Тег удален!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        TagsView.Refresh();
                    }
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void ResetForm()
        {
            _isEditMode = false;
            _currentTag = new Tag();
            txtTagName.Text = "";
            btnSave.Content = "Сохранить";
            btnSave.IsEnabled = false;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}