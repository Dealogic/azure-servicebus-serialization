namespace Dealogic.ServiceBus.Azure.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Json body serializer
    /// </summary>
    /// <seealso cref="Dealogic.ServiceBus.Azure.Serialization.IBodySerializer"/>
    public class JsonBodySerializer : IBodySerializer
    {
        private const string contentType = "application/json";

        private readonly Lazy<JsonSerializer> serializer = new Lazy<JsonSerializer>();
        private readonly JsonSerializerSettings jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonBodySerializer"/> class.
        /// </summary>
        public JsonBodySerializer()
            : this(new JsonSerializerSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonBodySerializer"/> class.
        /// </summary>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <exception cref="ArgumentNullException">jsonSerializerSettings is null</exception>
        public JsonBodySerializer(JsonSerializerSettings jsonSerializerSettings)
        {
            this.jsonSerializerSettings = jsonSerializerSettings ?? throw new ArgumentNullException(nameof(jsonSerializerSettings));
            this.serializer = new Lazy<JsonSerializer>(() => JsonSerializer.Create(this.jsonSerializerSettings));
        }

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

            using (var innerStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(innerStream, Encoding.UTF8))
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    this.serializer.Value.Serialize(jsonWriter, body);
                }

                return innerStream.ToArray();
            }
        }
    }
}