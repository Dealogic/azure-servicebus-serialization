namespace Dealogic.ServiceBus.Azure.Serialization
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Compressed Xml body deserializer
    /// </summary>
    public class GZipXmlBodyDeserializer : IBodyDeserializer
    {
        private const string contentType = "application/xml";
        private const string contentEncoding = "gzip";

        private readonly XmlReaderSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="GZipXmlBodyDeserializer"/> class.
        /// </summary>
        public GZipXmlBodyDeserializer()
            : this(new XmlReaderSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GZipXmlBodyDeserializer"/> class.
        /// </summary>
        /// <param name="xmlReaderSettings">The XML reader settings.</param>
        /// <exception cref="ArgumentNullException">xmlReaderSettings is null</exception>
        public GZipXmlBodyDeserializer(XmlReaderSettings xmlReaderSettings)
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
        public string ContentEncoding => contentEncoding;

        /// <inheritdoc/>
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
            using (var zipStream = new GZipStream(sourceStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(zipStream, Encoding.UTF8))
            using (var xmlReader = XmlReader.Create(reader, this.settings))
            {
                return new DataContractSerializer(bodyType).ReadObject(xmlReader);
            }
        }
    }
}