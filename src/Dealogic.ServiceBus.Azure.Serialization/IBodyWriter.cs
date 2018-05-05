namespace Dealogic.ServiceBus.Azure.Serialization
{
    using Microsoft.Azure.ServiceBus;

    /// <summary>
    /// Body writer interface
    /// </summary>
    public interface IBodyWriter
    {
        /// <summary>
        /// Gets the default body serializer.
        /// </summary>
        /// <value>The default body serializer.</value>
        IBodySerializer DefaultBodySerializer { get; }

        /// <summary>
        /// Writes the body.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="body">The body.</param>
        void WriteBody(Message message, object body);

        /// <summary>
        /// Writes the body.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="body">The body.</param>
        /// <param name="bodySerializer">The body serializer.</param>
        void WriteBody(Message message, object body, IBodySerializer bodySerializer);
    }
}