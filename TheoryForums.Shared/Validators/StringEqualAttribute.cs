using System.ComponentModel.DataAnnotations;

namespace TheoryForums.Shared.Validators
{
    internal class StringEqualAttribute : ValidationAttribute
    {
        private readonly string _OtherProperty;
        public override bool RequiresValidationContext { get => true; }

        public StringEqualAttribute(string otherProperty)
        {
            _OtherProperty = otherProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_OtherProperty);

            if (property == null)
            {
                return new ValidationResult(string.Format("Unknown property: {0}", _OtherProperty));
            }

            var otherValue = property.GetValue(validationContext.ObjectInstance, null);

            if (((string)value).ToLower() == ((string)otherValue).ToLower())
            {
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }

            return null;
        }

    }
}
