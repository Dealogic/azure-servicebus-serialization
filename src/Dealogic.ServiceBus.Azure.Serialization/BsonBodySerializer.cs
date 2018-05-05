namespace Dealogic.ServiceBus.Azure.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;

    /// <summary>
    /// Bson body serializer
    /// </summary>
    /// <seealso cref="Dealogic.ServiceBus.Azure.Serialization.IBodySerializer"/>
    public class BsonBodySerializer : IBodySerializer
    {
        private const string mediaType = "application/bson";

        private readonly Lazy<JsonSerializer> serializer = new Lazy<JsonSerializer>();

        private readonly JsonSerializerSettings jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonBodySerializer"/> class.
        /// </summary>
        public BsonBodySerializer()
            : this(new JsonSerializerSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonBodySerializer"/> class.
        /// </summary>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <exception cref="ArgumentNullException">jsonSerializerSettings is null</exception>
        public BsonBodySerializer(JsonSerializerSettings jsonSerializerSettings)
        {
            this.jsonSerializerSettings = jsonSerializerSettings ?? throw new ArgumentNullException(nameof(jsonSerializerSettings));
            this.serializer = new Lazy<JsonSerializer>(() => JsonSerializer.Create(this.jsonSerializerSettings));
        }

        /// <inheritdoc/>
        public ContentType ContentType => new ContentType(mediaType);

        /// <inheritdoc/>
        public string ContentEncoding => null;

        /// <inheritdoc/>
        public virtual byte[] Serialize(object body)
        {
            if (body == null)
            {
                return null;
            }

            using (var innerStream = new MemoryStream())
            using (var writer = new BsonDataWriter(innerStream))
            {
                this.serializer.Value.Serialize(writer, body);
                writer.Flush();
                return innerStream.ToArray();
            }
        }
    }
}