using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.Validators
{
    public sealed class NonNegativeIntegerAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string valueString = value is null ? "" : (string)value;

            if (!int.TryParse(valueString, out int valueInt))
            {
                return new("Enter a integer number.");
            }

            if (valueInt < 0)
            {
                return new("Enter a non negative integer number.");
            }

            return ValidationResult.Success!;
        }
    }
}
