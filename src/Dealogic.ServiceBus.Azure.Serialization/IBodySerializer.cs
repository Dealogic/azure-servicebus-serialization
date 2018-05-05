namespace Dealogic.ServiceBus.Azure.Serialization
{
    using System.Net.Mime;

    /// <summary>
    /// Body serializer interface
    /// </summary>
    public interface IBodySerializer
    {
        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        ContentType ContentType { get; }

        /// <summary>
        /// Gets the content encoding.
        /// </summary>
        /// <value>The content encoding.</value>
        string ContentEncoding { get; }

        /// <summary>
        /// Serializes the specified body.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <returns>Serialized body.</returns>
        byte[] Serialize(object body);
    }
}