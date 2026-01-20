using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement.Common.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when the system fails to lend an entity.
    /// </summary>
    public class FailedToLendException : Exception
    {
        public Type EntityType { get; }
        public string? Reason { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToLendException"/> class
        /// with the specified entity type and optional reason.
        /// </summary>
        /// <param name="entityType">The type of the entity that could not be lent.</param>
        /// <param name="reason">An optional description of the reason for failure.</param>
        public FailedToLendException(Type entityType, string? reason = null)
            : base(BuildMessage(entityType, reason))
        {
            EntityType = entityType;
            Reason = reason;
        }

        /// <summary>
        /// Builds the exception message based on the entity type and optional reason.
        /// </summary>
        /// <param name="entityType">The type of the entity.</param>
        /// <param name="reason">An optional reason for the failure.</param>
        /// <returns>A formatted exception message.</returns>
        private static string BuildMessage(Type entityType, string? reason)
        {
            return reason is null
                ? $"Failed to lend a entity of type '{entityType.Name}'."
                : $"Failed to create entity of type '{entityType.Name}'. Reason: {reason}";
        }
    }
}
