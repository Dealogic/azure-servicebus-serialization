namespace Dealogic.ServiceBus.Azure.Serialization
{
    using System;
    using Microsoft.Azure.ServiceBus;

    internal static class MessageExtensions
    {
        public static string GetContentEncoding(this Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.UserProperties.TryGetValue(CustomPropertyNames.ContentEncodingUserPropertyName, out object encoding) && encoding is string encodingString)
            {
                return encodingString?.ToLowerInvariant();
            }

            return null;
        }
    }
}