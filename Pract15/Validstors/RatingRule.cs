using System.Globalization;
using System.Windows.Controls;

namespace Pract15.Validstors
{
    internal class RatingRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // Пусто — допустимо (ошибка появится только если что-то ввели)
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.ValidResult;

            // Разрешаем дробные числа (через точку и запятую)
            if (!double.TryParse(
            value.ToString().Replace(',', '.'),
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out double rating))
            {
                return new ValidationResult(false, "Введите число от 0 до 5");
            }


            if (rating < 0 || rating > 5)
                return new ValidationResult(false, "Рейтинг должен быть от 0 до 5");

            return ValidationResult.ValidResult;
        }
    }
}
