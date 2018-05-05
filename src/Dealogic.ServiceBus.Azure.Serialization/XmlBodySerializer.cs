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
    /// Xml body serializer
    /// </summary>
    /// <seealso cref="Dealogic.ServiceBus.Azure.Serialization.IBodySerializer"/>
    public class XmlBodySerializer : IBodySerializer
    {
        private const string contentType = "application/xml";

        /// <summary>
        /// The XML writer settings
        /// </summary>
        private readonly XmlWriterSettings xmlWriterSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlBodySerializer"/> class.
        /// </summary>
        public XmlBodySerializer()
            : this(new XmlWriterSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlBodySerializer"/> class.
        /// </summary>
        /// <param name="xmlWriterSettings">The XML writer settings.</param>
        /// <exception cref="System.ArgumentNullException">xmlWriterSettings is null</exception>
        public XmlBodySerializer(XmlWriterSettings xmlWriterSettings)
        {
            this.xmlWriterSettings = xmlWriterSettings ?? throw new ArgumentNullException(nameof(xmlWriterSettings));
        }

        /// <summary>
        /// Gets or sets the compression level.
        /// </summary>
        /// <value>The compression level.</value>
        public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Fastest;

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

            var serializer = new DataContractSerializer(body.GetType());

            using (var targetStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(targetStream, Encoding.UTF8))
                using (var xmlWriter = XmlWriter.Create(streamWriter))
                {
                    serializer.WriteObject(xmlWriter, body);
                }

                return targetStream.ToArray();
            }
        }
    }
}