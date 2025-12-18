using System;
using System.Globalization;
using System.Windows.Controls;

namespace Pract15.Validstors
{
    public class TagValid : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string input = value?.ToString() ?? "";

            // Если пусто - ошибка (даже при фокусе)
            if (string.IsNullOrWhiteSpace(input))
            {
                return new ValidationResult(false, "Название тега обязательно");
            }

            return ValidationResult.ValidResult;
        }
    }
}