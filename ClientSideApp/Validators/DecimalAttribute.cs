using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.Validators
{
    public sealed class DecimalAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string price = value is null ? "" : (string)value;

            if (!decimal.TryParse(price, out decimal priceDec))
            {
                return new("Enter a real number.");
            }

            int decimalIndex = price.IndexOf(',');
            int digitsBeforeDecimal = decimalIndex;
            int digitsAfterDecimal = price.Length - decimalIndex - 1;

            if (digitsAfterDecimal > 2 || digitsBeforeDecimal > 18 || priceDec < 0)
            {
                return new("Enter a real non negative number (maximum 2 numbers after the decimal point, maximum 18 numbers before the decimal point).");
            }

            return ValidationResult.Success;
        }
    }
}
