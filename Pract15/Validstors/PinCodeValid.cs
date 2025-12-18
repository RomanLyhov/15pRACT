using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Pract15.Validstors
{
    public class PinCodeValid : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(false, "Введите пин-код");
            }

            string pin = value.ToString();

            if (pin != "1234")
                return new ValidationResult(false, "Неверный пин-код");

            return ValidationResult.ValidResult;
        }
    }
}