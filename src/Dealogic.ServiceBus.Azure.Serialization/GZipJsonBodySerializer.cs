namespace Dealogic.ServiceBus.Azure.Serialization
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Net.Mime;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Json body serializer
    /// </summary>
    /// <seealso cref="Dealogic.ServiceBus.Azure.Serialization.IBodySerializer"/>
    public class GZipJsonBodySerializer : IBodySerializer
    {
        private const string contentType = "application/json";
        private const string contentEncoding = "gzip";

        private readonly Lazy<JsonSerializer> serializer = new Lazy<JsonSerializer>();
        private readonly JsonSerializerSettings jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="GZipJsonBodySerializer"/> class.
        /// </summary>
        public GZipJsonBodySerializer()
            : this(new JsonSerializerSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GZipJsonBodySerializer"/> class.
        /// </summary>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <exception cref="ArgumentNullException">jsonSerializerSettings is null</exception>
        public GZipJsonBodySerializer(JsonSerializerSettings jsonSerializerSettings)
        {
            this.jsonSerializerSettings = jsonSerializerSettings ?? throw new ArgumentNullException(nameof(jsonSerializerSettings));
            this.serializer = new Lazy<JsonSerializer>(() => JsonSerializer.Create(this.jsonSerializerSettings));
        }

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

            using (var innerStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(innerStream, CompressionLevel.Optimal))
                using (var streamWriter = new StreamWriter(gzipStream, Encoding.UTF8))
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    this.serializer.Value.Serialize(jsonWriter, body);
                }

                return innerStream.ToArray();
            }
        }
    }
}