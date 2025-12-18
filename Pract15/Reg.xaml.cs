using Microsoft.IdentityModel.Tokens;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Pract15
{
    public partial class Reg : Page
    {
        private const string ManagerPin = "1234";

        public Reg()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Glavn(false));
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        { 
            // Скрываем ошибку
            errorBorder.Visibility = Visibility.Collapsed;

            // Создаем валидатор
            var validator = new Validstors.PinCodeValid();
            var result = validator.Validate(PasswordBox2.Password, System.Globalization.CultureInfo.CurrentCulture);

            if (!result.IsValid)
            {
                // Показываем ошибку
                errorText.Text = result.ErrorContent.ToString();
                errorBorder.Visibility = Visibility.Visible;
                return;
            }

            // Если валидация прошла
            if (PasswordBox2.Password == ManagerPin)
            {
                MessageBox.Show("Вы вошли как менеджер!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new Glavn(true));
            }
            else
            {
                errorText.Text = "Неверный пин-код! Попробуйте снова.";
                errorBorder.Visibility = Visibility.Visible;
                PasswordBox2.Clear();
                PasswordBox2.Focus();
            }
        }
    }
}
