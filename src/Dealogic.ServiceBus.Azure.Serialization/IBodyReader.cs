namespace Dealogic.ServiceBus.Azure.Serialization
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Azure.ServiceBus;

    /// <summary>
    /// Body reader interface
    /// </summary>
    public interface IBodyReader
    {
        /// <summary>
        /// Gets the registered deserializers.
        /// </summary>
        /// <value>The registered deserializers.</value>
        IEnumerable<IBodyDeserializer> RegisteredDeserializers { get; }

        /// <summary>
        /// Reads the body.
        /// </summary>
        /// <typeparam name="T">The type of the body.</typeparam>
        /// <param name="message">The message.</param>
        /// <returns>The body.</returns>
        T ReadBody<T>(Message message);

        /// <summary>
        /// Reads the body.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="bodyType">Type of the body.</param>
        /// <returns>The deserialized body.</returns>
        object ReadBody(Message message, Type bodyType);

        /// <summary>
        /// Registers the deserializer.
        /// </summary>
        /// <param name="bodyDeserializer">The body deserializer.</param>
        void RegisterDeserializer(IBodyDeserializer bodyDeserializer);
    }
}