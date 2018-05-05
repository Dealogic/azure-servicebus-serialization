namespace Dealogic.ServiceBus.Azure.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;

    /// <summary>
    /// Bson Body deserializer
    /// </summary>
    /// <seealso cref="Dealogic.ServiceBus.Azure.Serialization.IBodyDeserializer"/>
    public class BsonBodyDeserializer : IBodyDeserializer
    {
        private const string mediaType = "application/bson";

        private readonly Lazy<JsonSerializer> serializer = new Lazy<JsonSerializer>();

        private readonly JsonSerializerSettings jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonBodyDeserializer"/> class.
        /// </summary>
        public BsonBodyDeserializer()
            : this(new JsonSerializerSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonBodyDeserializer"/> class.
        /// </summary>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <exception cref="ArgumentNullException">jsonSerializerSettings is null</exception>
        public BsonBodyDeserializer(JsonSerializerSettings jsonSerializerSettings)
        {
            this.jsonSerializerSettings = jsonSerializerSettings ?? throw new ArgumentNullException(nameof(jsonSerializerSettings));
            this.serializer = new Lazy<JsonSerializer>(() => JsonSerializer.Create(this.jsonSerializerSettings));
        }

        /// <inheritdoc/>
        public ContentType ContentType => new ContentType(mediaType);

        /// <inheritdoc/>
        public string ContentEncoding => null;

        /// <inheritdoc/>
        public virtual T Deserialize<T>(byte[] body)
        {
            if (body == null)
            {
                return default(T);
            }

            using (var innerStream = new MemoryStream(body, false))
            using (var reader = new BsonDataReader(innerStream))
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
            using (var reader = new BsonDataReader(innerStream))
            {
                return serializer.Value.Deserialize(reader, bodyType);
            }
        }
    }
}