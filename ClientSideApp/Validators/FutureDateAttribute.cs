using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.Validators
{
    public sealed class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            DateTime date = value is null ? DateTime.Now : (DateTime)value;

            if(date < DateTime.Now) 
            {
                return new(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
