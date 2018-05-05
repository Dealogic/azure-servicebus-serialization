namespace Dealogic.ServiceBus.Azure.Serialization
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Net.Mime;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Json body deserializer
    /// </summary>
    /// <seealso cref="Dealogic.ServiceBus.Azure.Serialization.IBodyDeserializer"/>
    public class GZipJsonBodyDeserializer : IBodyDeserializer
    {
        private const string contentType = "application/json";
        private const string contentEncoding = "gzip";

        private readonly Lazy<JsonSerializer> serializer = new Lazy<JsonSerializer>();
        private readonly JsonSerializerSettings jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonBodyDeserializer"/> class.
        /// </summary>
        public GZipJsonBodyDeserializer()
            : this(new JsonSerializerSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonBodyDeserializer"/> class.
        /// </summary>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <exception cref="ArgumentNullException">jsonSerializerSettings is null</exception>
        public GZipJsonBodyDeserializer(JsonSerializerSettings jsonSerializerSettings)
        {
            this.jsonSerializerSettings = jsonSerializerSettings ?? throw new ArgumentNullException(nameof(jsonSerializerSettings));
            this.serializer = new Lazy<JsonSerializer>(() => JsonSerializer.Create(this.jsonSerializerSettings));
        }

        /// <inheritdoc/>
        public ContentType ContentType => new ContentType(contentType);

        /// <inheritdoc/>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

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
            using (var streamReader = new StreamReader(gzipStream, this.Encoding))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return this.serializer.Value.Deserialize<T>(jsonTextReader);
            }
        }

        /// <summary>
        /// Deserializes the specified body.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="bodyType">Type of the body.</param>
        /// <returns>Deserialized body.</returns>
        /// <exception cref="ArgumentNullException">bodyType is null</exception>
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
            using (var streamReader = new StreamReader(gzipStream, this.Encoding))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return this.serializer.Value.Deserialize(jsonTextReader, bodyType);
            }
        }
    }
}