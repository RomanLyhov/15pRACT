using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Pract15.Validstors
{
    public class IntPositiveRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!int.TryParse(value?.ToString(), out int result))
                return new ValidationResult(false, "Введите целое число");

            if (result < 0)
                return new ValidationResult(false, "Количество не может быть отрицательным");

            return ValidationResult.ValidResult;
        }
    }
}