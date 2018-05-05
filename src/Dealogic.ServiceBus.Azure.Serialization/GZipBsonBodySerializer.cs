namespace Dealogic.ServiceBus.Azure.Serialization
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Net.Mime;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;

    /// <summary>
    /// GZip Bson body serializer
    /// </summary>
    /// <seealso cref="Dealogic.ServiceBus.Azure.Serialization.IBodySerializer"/>
    public class GZipBsonBodySerializer : IBodySerializer
    {
        private const string contentType = "application/bson";
        private const string contentEncoding = "gzip";

        private readonly Lazy<JsonSerializer> serializer = new Lazy<JsonSerializer>();

        private readonly JsonSerializerSettings jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonBodySerializer"/> class.
        /// </summary>
        public GZipBsonBodySerializer()
            : this(new JsonSerializerSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonBodySerializer"/> class.
        /// </summary>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <exception cref="ArgumentNullException">jsonSerializerSettings is null</exception>
        public GZipBsonBodySerializer(JsonSerializerSettings jsonSerializerSettings)
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
                using (var writer = new BsonDataWriter(gzipStream))
                {
                    this.serializer.Value.Serialize(writer, body);
                }

                return innerStream.ToArray();
            }
        }
    }
}