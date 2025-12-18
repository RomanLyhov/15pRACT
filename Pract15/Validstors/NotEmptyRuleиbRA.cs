using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Pract15.Validstors
{
    public class NotEmptyRuleиbRA : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string text = value as string ?? "";

            if (string.IsNullOrWhiteSpace(text))
                return new ValidationResult(false, "Название бренда не может быть пустым");

            return ValidationResult.ValidResult;
        }
    }
}
