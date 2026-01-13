using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement.Common.Attributes
{
    /// <summary>
    /// Validates that a numeric year value falls within a specified range.
    /// </summary>
    /// <remarks>
    /// This attribute ensures that the value is between a minimum year (default 0)
    /// and the current year. Typically applied to properties like <c>PublicationYear</c>.
    /// </remarks>
    public class MaxYearValidationAttribute : ValidationAttribute
    {
        private readonly int _min;

        public MaxYearValidationAttribute(int min = 0)
        {
            _min = min;
        }

        /// <summary>
        /// Determines whether the specified value is valid.
        /// </summary>
        /// <param name="value">The value of the property being validated.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>
        /// A <see cref="ValidationResult"/> indicating success or failure.
        /// Returns a validation error if the value is less than the minimum or greater than the current year.
        /// </returns>
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            if (value is short year)
            {
                int currentYear = DateTime.Now.Year;
                if (year < _min || year > currentYear)
                {
                    return new ValidationResult(
                        $"PublicationYear must be between {_min} and {currentYear}."
                    );
                }
            }

            return ValidationResult.Success;
        }
    }
}
