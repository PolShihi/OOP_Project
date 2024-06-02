using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.Validators
{
    public sealed class LaterThanAttribute : ValidationAttribute
    {
        public LaterThanAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            object instance = validationContext.ObjectInstance;
            object otherValue = instance.GetType().GetProperty(PropertyName).GetValue(instance);

            DateTime dateLater = (DateTime)value;
            DateTime dateBefore = (DateTime)otherValue;

            if (dateLater <= dateBefore)
            {
                return new(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
