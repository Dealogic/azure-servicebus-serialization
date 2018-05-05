namespace Dealogic.ServiceBus.Azure.Serialization
{
    using System;
    using Dealogic.ServiceBus.Azure.Serialization.Tracing;
    using Microsoft.Azure.ServiceBus;

    /// <summary>
    /// Body writer
    /// </summary>
    public class BodyWriter : IBodyWriter
    {
        private static readonly Lazy<BodyWriter> singletonFactory = new Lazy<BodyWriter>(() => new BodyWriter());

        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <value>The default.</value>
        public static BodyWriter Default => singletonFactory.Value;

        /// <summary>
        /// Gets the default body serializer.
        /// </summary>
        /// <value>The default body serializer.</value>
        public IBodySerializer DefaultBodySerializer { get; set; } = new BsonBodySerializer();

        /// <inheritdoc/>
        public virtual void WriteBody(Message message, object body)
        {
            if (message == null || body == null)
            {
                return;
            }

            this.WriteBody(message, body, this.DefaultBodySerializer);
        }

        /// <summary>
        /// Writes the body.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="body">The body.</param>
        /// <param name="bodySerializer">The body serializer.</param>
        /// <exception cref="System.ArgumentNullException">bodySerializer is null.</exception>
        public virtual void WriteBody(Message message, object body, IBodySerializer bodySerializer)
        {
            if (message == null || body == null)
            {
                return;
            }

            if (bodySerializer == null)
            {
                throw new ArgumentNullException(nameof(bodySerializer));
            }

            ServiceBusSerializationEventSource.Log.UsingSerializer(bodySerializer.GetType().FullName);
            message.Body = bodySerializer.Serialize(body);
            message.ContentType = bodySerializer.ContentType.MediaType;

            if (!message.UserProperties.ContainsKey(CustomPropertyNames.MessageTypeUserPropertyName))
            {
                message.UserProperties[CustomPropertyNames.MessageTypeUserPropertyName] = body.GetType().FullName;
            }

            if (bodySerializer.ContentEncoding != null && !message.UserProperties.ContainsKey(CustomPropertyNames.ContentEncodingUserPropertyName))
            {
                ServiceBusSerializationEventSource.Log.SetContentEncoding(bodySerializer.ContentEncoding);
                message.UserProperties[CustomPropertyNames.ContentEncodingUserPropertyName] = bodySerializer.ContentEncoding;
            }

            ServiceBusSerializationEventSource.Log.SetBodyContentType(bodySerializer.ContentType.MediaType);
        }
    }
}