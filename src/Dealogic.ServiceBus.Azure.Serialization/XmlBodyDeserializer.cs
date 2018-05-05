namespace Dealogic.ServiceBus.Azure.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Xml Body deserializer
    /// </summary>
    public class XmlBodyDeserializer : IBodyDeserializer
    {
        private const string contentType = "application/xml";

        private readonly XmlReaderSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlBodyDeserializer"/> class.
        /// </summary>
        public XmlBodyDeserializer()
            : this(new XmlReaderSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlBodyDeserializer"/> class.
        /// </summary>
        /// <param name="xmlReaderSettings">The XML reader settings.</param>
        public XmlBodyDeserializer(XmlReaderSettings xmlReaderSettings)
        {
            this.settings = xmlReaderSettings ?? throw new ArgumentNullException(nameof(xmlReaderSettings));
        }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <inheritdoc/>
        public ContentType ContentType => new ContentType(contentType);

        /// <inheritdoc/>
        public string ContentEncoding => null;

        /// <summary>
        /// Deserializes the specified body.
        /// </summary>
        /// <typeparam name="T">The body type.</typeparam>
        /// <param name="body">The body.</param>
        /// <returns>Deserialized body.</returns>
        public virtual T Deserialize<T>(byte[] body)
        {
            if (body == null)
            {
                return default(T);
            }

            return (T)this.Deserialize(body, typeof(T));
        }

        /// <summary>
        /// Deserializes the specified body.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="bodyType">Type of the body.</param>
        /// <returns>Deserialized body.</returns>
        /// <exception cref="System.ArgumentNullException">bodyType is null</exception>
        public virtual object Deserialize(byte[] body, Type bodyType)
        {
            if (body == null)
            {
                return null;
            }

            if (bodyType == null)
            {
                throw new ArgumentNullException(nameof(bodyType));
            }

            using (var sourceStream = new MemoryStream(body, false))
            using (var reader = new StreamReader(sourceStream, this.Encoding))
            using (var xmlReader = XmlReader.Create(reader, this.settings))
            {
                return new DataContractSerializer(bodyType).ReadObject(xmlReader);
            }
        }
    }
}