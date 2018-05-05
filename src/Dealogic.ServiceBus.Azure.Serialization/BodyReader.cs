namespace Dealogic.ServiceBus.Azure.Serialization
{
    using System;
    using System.Collections.Generic;
    using Dealogic.ServiceBus.Azure.Serialization.Tracing;
    using Microsoft.Azure.ServiceBus;

    /// <summary>
    /// Default message body reader
    /// </summary>
    /// <seealso cref="Dealogic.ServiceBus.Azure.Serialization.IBodyReader"/>
    public class BodyReader : IBodyReader
    {
        private static readonly Lazy<BodyReader> singletonFactory = new Lazy<BodyReader>(() => new BodyReader());

        /// <summary>
        /// The registered deserializers
        /// </summary>
        private IDictionary<(string ContentType, string ContentEncoding), IBodyDeserializer> registeredDeserializers = new Dictionary<(string ContentType, string ContentEncoding), IBodyDeserializer>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BodyReader"/> class.
        /// </summary>
        public BodyReader()
        {
            this.RegisterDeserializer(new BsonBodyDeserializer());
            this.RegisterDeserializer(new JsonBodyDeserializer());
            this.RegisterDeserializer(new XmlBodyDeserializer());
            this.RegisterDeserializer(new GZipBsonBodyDeserializer());
            this.RegisterDeserializer(new GZipJsonBodyDeserializer());
            this.RegisterDeserializer(new GZipXmlBodyDeserializer());
            this.RegisterDeserializer(new BinaryDeserializer());
        }

        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <value>The default.</value>
        public static BodyReader Default => singletonFactory.Value;

        /// <inheritdoc/>
        public IEnumerable<IBodyDeserializer> RegisteredDeserializers => registeredDeserializers.Values;

        /// <summary>
        /// Reads the body.
        /// </summary>
        /// <typeparam name="T">Serialized message body type.</typeparam>
        /// <param name="message">The message.</param>
        /// <returns>The content</returns>
        /// <exception cref="System.ArgumentNullException">message is null</exception>
        /// <exception cref="System.InvalidOperationException">
        /// No deserializer was registerd for the content type
        /// </exception>
        public virtual T ReadBody<T>(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Body == null)
            {
                ServiceBusSerializationEventSource.Log.NoBodyFound();
                return default(T);
            }

            var contentEncoding = message.GetContentEncoding();
            if (this.registeredDeserializers.TryGetValue((message.ContentType?.ToLowerInvariant(), contentEncoding), out IBodyDeserializer bodyDeserializer))
            {
                ServiceBusSerializationEventSource.Log.UsingDeserializer(bodyDeserializer.GetType().FullName, message.ContentType, contentEncoding);
                return bodyDeserializer.Deserialize<T>(message.Body);
            }

            throw new InvalidOperationException($"No deserializer is registered for {message.ContentType} and {contentEncoding ?? "N/A"}");
        }

        /// <summary>
        /// Reads the body.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="bodyType">Type of the body.</param>
        /// <returns>Deserialized body.</returns>
        /// <exception cref="System.ArgumentNullException">message or bodyType is null</exception>
        /// <exception cref="System.InvalidOperationException">
        /// No serialier was registerd for the content type
        /// </exception>
        public virtual object ReadBody(Message message, Type bodyType)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Body == null)
            {
                ServiceBusSerializationEventSource.Log.NoBodyFound();
                return null;
            }

            if (bodyType == null)
            {
                throw new ArgumentNullException(nameof(bodyType));
            }

            var contentEncoding = message.GetContentEncoding();
            if (this.registeredDeserializers.TryGetValue((message.ContentType?.ToLowerInvariant(), contentEncoding), out IBodyDeserializer bodyDeserializer))
            {
                ServiceBusSerializationEventSource.Log.UsingDeserializer(bodyDeserializer.GetType().FullName, message.ContentType, contentEncoding);
                return bodyDeserializer.Deserialize(message.Body, bodyType);
            }

            throw new InvalidOperationException($"No deserializer is registered for {message.ContentType} and {contentEncoding ?? "N/A"}");
        }

        /// <summary>
        /// Registers the deserializer.
        /// </summary>
        /// <param name="bodyDeserializer">The body deserializer.</param>
        /// <exception cref="System.ArgumentNullException">bodyDeserializer is null.</exception>
        public virtual void RegisterDeserializer(IBodyDeserializer bodyDeserializer)
        {
            if (bodyDeserializer == null)
            {
                throw new ArgumentNullException(nameof(bodyDeserializer));
            }

            this.registeredDeserializers[(bodyDeserializer.ContentType.MediaType.ToLowerInvariant(), bodyDeserializer.ContentEncoding?.ToLowerInvariant())] = bodyDeserializer;
            ServiceBusSerializationEventSource.Log.RegisterDeserializer(bodyDeserializer.ContentType.MediaType, bodyDeserializer.GetType().FullName, bodyDeserializer.ContentEncoding);
        }
    }
}