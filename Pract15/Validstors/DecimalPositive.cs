using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Pract15.Validstors
{
    public class DecimalPositive : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!decimal.TryParse(value?.ToString(), out decimal result))
                return new ValidationResult(false, "Введите корректное число");

            if (result <= 0)
                return new ValidationResult(false, "Цена должна быть больше 0");

            return ValidationResult.ValidResult;
        }
    }
}
