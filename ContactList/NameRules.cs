using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ContactList
{
    public class NameRules: ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!(value is string)) return new ValidationResult(false, "Wrong Value");
            string name = (string)value;
            if (string.IsNullOrEmpty(name)) return new ValidationResult(false, "Empty Value");
            return ValidationResult.ValidResult;
        }
    }

    public class NumberRules : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!(value is string)) return new ValidationResult(false, "Wrong Value");
            string number = (string)value;
            Regex numberRegex = new Regex("\\d+");
            if (!numberRegex.IsMatch(number)) return new ValidationResult(false, "Invalid input");
            return ValidationResult.ValidResult;
        }
    }
}
