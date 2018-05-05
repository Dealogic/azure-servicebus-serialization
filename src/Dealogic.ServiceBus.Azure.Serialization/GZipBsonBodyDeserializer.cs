namespace Dealogic.ServiceBus.Azure.Serialization
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Net.Mime;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;

    /// <summary>
    /// GZip BSON Body deserializer
    /// </summary>
    /// <seealso cref="Dealogic.ServiceBus.Azure.Serialization.IBodyDeserializer"/>
    public class GZipBsonBodyDeserializer : IBodyDeserializer
    {
        private const string contentType = "application/bson";
        private const string contentEncoding = "gzip";

        private readonly Lazy<JsonSerializer> serializer = new Lazy<JsonSerializer>();

        private readonly JsonSerializerSettings jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="GZipBsonBodyDeserializer"/> class.
        /// </summary>
        public GZipBsonBodyDeserializer()
            : this(new JsonSerializerSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GZipBsonBodyDeserializer"/> class.
        /// </summary>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <exception cref="ArgumentNullException">jsonSerializerSettings is null</exception>
        public GZipBsonBodyDeserializer(JsonSerializerSettings jsonSerializerSettings)
        {
            this.jsonSerializerSettings = jsonSerializerSettings ?? throw new ArgumentNullException(nameof(jsonSerializerSettings));
            this.serializer = new Lazy<JsonSerializer>(() => JsonSerializer.Create(this.jsonSerializerSettings));
        }

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

            using (var innerStream = new MemoryStream(body, false))
            using (var gzipStream = new GZipStream(innerStream, CompressionMode.Decompress))
            using (var reader = new BsonDataReader(gzipStream))
            {
                return serializer.Value.Deserialize<T>(reader);
            }
        }

        /// <summary>
        /// Deserializes the specified body.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="bodyType">Type of the body.</param>
        /// <returns>The deserialized body</returns>
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

            using (var innerStream = new MemoryStream(body, false))
            using (var gzipStream = new GZipStream(innerStream, CompressionMode.Decompress))
            using (var reader = new BsonDataReader(gzipStream))
            {
                return serializer.Value.Deserialize(reader, bodyType);
            }
        }
    }
}