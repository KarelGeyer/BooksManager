using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement.Common.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when the system fails to create an entity.
    /// </summary>
    public class FailedToCreateException : Exception
    {
        public Type EntityType { get; }
        public string? Reason { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToCreateException"/> class
        /// with the specified entity type and optional reason.
        /// </summary>
        /// <param name="entityType">The type of the entity that could not be created.</param>
        /// <param name="reason">An optional description of the reason for failure.</param>
        public FailedToCreateException(Type entityType, string? reason = null)
            : base(BuildMessage(entityType, reason))
        {
            EntityType = entityType;
            Reason = reason;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToCreateException"/> class
        /// with the specified entity type, optional reason, and inner exception.
        /// </summary>
        /// <param name="entityType">The type of the entity that could not be created.</param>
        /// <param name="reason">An optional description of the reason for failure.</param>
        /// <param name="innerException">The exception that caused the current exception.</param>
        public FailedToCreateException(Type entityType, string? reason, Exception innerException)
            : base(BuildMessage(entityType, reason), innerException)
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
                ? $"Failed to create entity of type '{entityType.Name}'."
                : $"Failed to create entity of type '{entityType.Name}'. Reason: {reason}";
        }
    }
}
