namespace Dealogic.ServiceBus.Azure.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Binary serializer
    /// </summary>
    /// <seealso cref="Dealogic.ServiceBus.Azure.Serialization.IBodySerializer"/>
    public class BinarySerializer : IBodySerializer
    {
        private const string contentType = "applicaiton/octet-stream";

        /// <summary>
        /// The formatter factory
        /// </summary>
        private readonly Lazy<BinaryFormatter> formatterFactory = new Lazy<BinaryFormatter>(() => new BinaryFormatter());

        /// <inheritdoc/>
        public ContentType ContentType => new ContentType(contentType);

        /// <inheritdoc/>
        public string ContentEncoding => null;

        /// <inheritdoc/>
        public virtual byte[] Serialize(object body)
        {
            if (body == null)
            {
                return null;
            }

            using (var innerStream = new MemoryStream())
            {
                formatterFactory.Value.Serialize(innerStream, body);
                return innerStream.ToArray();
            }
        }
    }
}