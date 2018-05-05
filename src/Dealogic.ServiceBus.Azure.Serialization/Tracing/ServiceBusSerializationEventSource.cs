namespace Dealogic.ServiceBus.Azure.Serialization.Tracing
{
    using System;
    using System.Diagnostics.Tracing;

    [EventSource(Name = Configuration.EventSourceName)]
    internal sealed partial class ServiceBusSerializationEventSource : EventSource
    {
        private static readonly Lazy<ServiceBusSerializationEventSource> Instance = new Lazy<ServiceBusSerializationEventSource>(() => new ServiceBusSerializationEventSource());

        /// <summary>
        /// Prevents a default instance of the <see cref="ServiceBusSerializationEventSource"/> class
        /// from being created.
        /// </summary>
        private ServiceBusSerializationEventSource()
        {
        }

        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <value>The log.</value>
        public static ServiceBusSerializationEventSource Log => Instance.Value;

        /// <summary>
        /// Registers the deserializer.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="type">The type.</param>
        /// <param name="contentEncoding">The content encoding</param>
        [Event(1, Level = EventLevel.Verbose, Message = "Deserializer {1} registered for {0}. Content encoding {2}")]
        public void RegisterDeserializer(string contentType, string type, string contentEncoding)
        {
            if (this.IsEnabled(EventLevel.Verbose, Keywords.All))
            {
                this.WriteEvent(1, contentType, type, contentEncoding);
            }
        }

        /// <summary>
        /// Usings the deserializer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="contentEncoding">Content encoding</param>
        [Event(2, Level = EventLevel.Verbose, Message = "Deserialize body using {0}")]
        public void UsingDeserializer(string type, string contentType, string contentEncoding)
        {
            if (this.IsEnabled(EventLevel.Verbose, Keywords.All))
            {
                this.WriteEvent(2, type, contentType, contentEncoding);
            }
        }

        /// <summary>
        /// Usings the serializer.
        /// </summary>
        /// <param name="type">The type.</param>
        [Event(3, Level = EventLevel.Verbose, Message = "Serialize body using {0}")]
        public void UsingSerializer(string type)
        {
            if (this.IsEnabled(EventLevel.Verbose, Keywords.All))
            {
                this.WriteEvent(3, type);
            }
        }

        /// <summary>
        /// Sets the type of the body content.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        [Event(4, Level = EventLevel.Verbose, Message = "Set body content type to {0}")]
        public void SetBodyContentType(string contentType)
        {
            if (this.IsEnabled(EventLevel.Verbose, Keywords.All))
            {
                this.WriteEvent(4, contentType);
            }
        }

        /// <summary>
        /// Sets the content encoding.
        /// </summary>
        /// <param name="contentEncoding">The content encoding.</param>
        [Event(5, Level = EventLevel.Verbose, Message = "Set content encoding to {0}")]
        public void SetContentEncoding(string contentEncoding)
        {
            if (this.IsEnabled(EventLevel.Verbose, Keywords.All))
            {
                this.WriteEvent(5, contentEncoding);
            }
        }

        /// <summary>
        /// Noes the body found.
        /// </summary>
        [Event(6, Level = EventLevel.Warning, Message = "No body found, skip deserialization")]
        public void NoBodyFound()
        {
            if (this.IsEnabled(EventLevel.Warning, Keywords.All))
            {
                this.WriteEvent(6);
            }
        }
    }
}