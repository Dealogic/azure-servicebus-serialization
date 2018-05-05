namespace Dealogic.ServiceBus.Azure.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Binary deserializer
    /// </summary>
    /// <seealso cref="Dealogic.ServiceBus.Azure.Serialization.IBodyDeserializer"/>
    public class BinaryDeserializer : IBodyDeserializer
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
        public virtual T Deserialize<T>(byte[] body)
        {
            if (body == null)
            {
                return default(T);
            }

            return (T)this.Deserialize(body, typeof(T));
        }

        /// <inheritdoc/>
        public virtual object Deserialize(byte[] body, Type bodyType)
        {
            if (body == null)
            {
                return null;
            }

            using (var innerStream = new MemoryStream(body, false))
            {
                return this.formatterFactory.Value.Deserialize(innerStream);
            }
        }
    }
}