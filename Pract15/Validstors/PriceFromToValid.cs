using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Pract15.Validstors
{
    public class PriceFromToValid : ValidationRule
    {
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (PriceFrom.HasValue && PriceTo.HasValue)
            {
                if (PriceFrom.Value > PriceTo.Value)
                {
                    return new ValidationResult(false, "Цена 'от' не может быть больше цены 'до'");
                }
            }

            return ValidationResult.ValidResult;
        }
    }
}
