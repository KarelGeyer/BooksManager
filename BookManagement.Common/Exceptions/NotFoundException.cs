using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement.Common.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when a requested entity is not found.
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class
        /// for the specified entity type and optional key.
        /// </summary>
        /// <param name="entityType">The type of the entity that was not found.</param>
        /// <param name="key">An optional key identifying the entity.</param>
        public NotFoundException(Type entityType, object? key = null)
            : base(
                key != null
                    ? $"{entityType.Name} with key '{key}' not found."
                    : $"{entityType.Name} not found."
            ) { }
    }
}
