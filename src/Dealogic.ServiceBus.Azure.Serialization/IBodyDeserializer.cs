namespace Dealogic.ServiceBus.Azure.Serialization
{
    using System;
    using System.Net.Mime;

    /// <summary>
    /// Body deserializer interface
    /// </summary>
    public interface IBodyDeserializer
    {
        /// <summary>
        /// Gets the type of the body content.
        /// </summary>
        /// <value>The type of the body content.</value>
        ContentType ContentType { get; }

        /// <summary>
        /// Gets or sets the content encoding.
        /// </summary>
        /// <value>The content encoding.</value>
        string ContentEncoding { get; }

        /// <summary>
        /// Deserializes the specified body.
        /// </summary>
        /// <typeparam name="T">The body type.</typeparam>
        /// <param name="body">The body.</param>
        /// <returns>Deserialized body.</returns>
        T Deserialize<T>(byte[] body);

        /// <summary>
        /// Deserializes the specified body.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="bodyType">Type of the body.</param>
        /// <returns>Deserialized body.</returns>
        object Deserialize(byte[] body, Type bodyType);
    }
}