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
    /// Compressed Xml body serializer
    /// </summary>
    /// <seealso cref="Dealogic.ServiceBus.Azure.Serialization.IBodySerializer"/>
    public class GZipXmlBodySerializer : IBodySerializer
    {
        private const string contentType = "application/xml";
        private const string contentEncoding = "gzip";

        /// <summary>
        /// The XML writer settings
        /// </summary>
        private readonly XmlWriterSettings xmlWriterSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="GZipXmlBodySerializer"/> class.
        /// </summary>
        public GZipXmlBodySerializer()
            : this(new XmlWriterSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GZipXmlBodySerializer"/> class.
        /// </summary>
        /// <param name="xmlWriterSettings">The XML writer settings.</param>
        /// <exception cref="ArgumentNullException">xmlWriterSettings is null</exception>
        public GZipXmlBodySerializer(XmlWriterSettings xmlWriterSettings)
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
        public string ContentEncoding => contentEncoding;

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
                using (var zipStream = new GZipStream(targetStream, this.CompressionLevel))
                using (var streamWriter = new StreamWriter(zipStream, Encoding.UTF8))
                using (var xmlWriter = XmlWriter.Create(streamWriter, this.xmlWriterSettings))
                {
                    serializer.WriteObject(xmlWriter, body);
                }

                return targetStream.ToArray();
            }
        }
    }
}