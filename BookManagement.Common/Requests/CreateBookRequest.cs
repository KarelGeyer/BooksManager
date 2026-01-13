using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookManagement.Common.Attributes;

namespace BookManagement.Common.Requests
{
    public class CreateBookRequest
    {
        [Required(ErrorMessage = "ISBN required")]
        [StringLength(
            17,
            MinimumLength = 14,
            ErrorMessage = "ISBN must be between 14 and 17 characters."
        )]
        public string ISBN { get; set; } = null!;

        [Required(ErrorMessage = "Title required.")]
        [StringLength(200, ErrorMessage = "Title cannot be longer than 200 characters.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Author required.")]
        [StringLength(100, ErrorMessage = "Author cannot be longer than 100 characters.")]
        public string Author { get; set; } = null!;

        [MaxYearValidation(1900)]
        public short PublicationYear { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "AmountAvailable must be non-negative.")]
        public short AmountAvailable { get; set; }
    }
}
