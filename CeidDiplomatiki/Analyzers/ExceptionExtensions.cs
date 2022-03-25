
using System;
using System.Text;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Extension methods for <see cref="Exception"/>
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Aggregates the <see cref="Exception.Message"/> in the <paramref name="exception"/> exception stack trace
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <returns></returns>
        public static string AggregateExceptionMessages(this Exception exception)
        {
            // If the exception is null...
            if (exception == null)
                // Return an empty string
                return string.Empty;

            // Initialize a string builder
            var builder = new StringBuilder();

            // Append the message
            builder.Append(exception.Message);

            // Get the inner exception
            var innerException = exception.InnerException;

            // While there is an inner exception...
            while (innerException != null)
            {
                // Append a comma
                builder.Append(", ");

                // Append the inner exception message
                builder.Append(innerException.Message);

                // Get the nested inner exception
                innerException = innerException.InnerException;
            }

            // Return the aggregated message
            return builder.ToString();
        }
    }
}
