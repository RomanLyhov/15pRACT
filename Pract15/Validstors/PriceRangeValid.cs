using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Pract15.Validstors
{
   public class PriceRangeValid : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // Поле может быть пустым
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.ValidResult;
            }

            string input = value.ToString().Replace(',', '.');

            if (!decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
            {
                return new ValidationResult(false, "Цена должна быть числом (например: 100 или 99.50)");
            }

            if (price < 0)
                return new ValidationResult(false, "Цена не может быть отрицательной");

            return ValidationResult.ValidResult;
        }
    }
}
