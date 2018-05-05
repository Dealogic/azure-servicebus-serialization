# Dealogic Azure Service Bus Serialization

Adds serialization support for Service Bus messages. By default it uses Bson serialization for message body.
When serializing messages the `ContentType` property will be populated with the serializers content type.
On deserialization the deserializer has to be registered with the reader with the correct content type to be able to deserialize.
You have to know the type of the body to read it back, the serializer does not track it.

## Content

* [Serializing messages](#serializing-messages)
* [Deserializing messages](#deserializing-messages)
* [Using custom serializers](#using-custom-serializers)
* [Tracing](#tracing)
* [Implementation notes](#implementation-notes)

### <a id="serializing-messages" /> Serializing messages

```csharp
var someObject = ...;
var message = new Message();
BodyWriter.Default.WriteBody(message, someObject);
```
### <a id="deserializing-messages" /> Deserializing messages

```csharp
SomeType body = (SomeType)BodyReader.Default.ReadBody(message, typeof(SomeType));
```
or with the generic method

```csharp
SomeType body = BodyReader.Default.ReadBody<SomeType>(message);
```
### <a id="using-custom-serializers" /> Using custom serializers

```csharp
var someObject = ...;
var message = new Message();
BodyWriter.Default.WriteBody(message, someObject, YourSerializerInstance);

BodyReader.Default.RegisterDeserializer(YourDeserializerInstance);
SomeType body = BodyReader.Default.ReadBody(message, typeof(SomeType));
```

### <a id="tracing" /> Tracing

The component publishes trace events using the eventsource model.
The name of the event source can be found in the `Dealogic.ServiceBus.Azure.Serialization.Tracing.Configuration.EventSourceName` constant.

## <a id="implementation-notes" /> Implementation notes

- On serialization the **ContentType** property of the message will be overwritten with the value set in the serializer
- On serialization a user property called **MessageType** will be set if not already exists with the [FullName](https://msdn.microsoft.com/en-us/library/system.type.fullname(v=vs.110).aspx) of the serialized object type
- On serialization a user property called **ContentEncoding** will be set if not already exists
- For deserializing the message a deserializer has to be registered in the BodyReader with the specified contenttype
- By default these deserializers are registered:
  1. BsonBodyDeserializer (application/bson) **(default serializer)**
  2. BinaryDeserializer (application/octet-stream)
  3. GZipBsonDeserializer (application/bson and gzip)
  4. XmlBodyDeserializer (application/xml)
  5. GZipXmlBodyDeserializer (application/xml and gzip)
  6. JsonBodyDeserializer (application/json)
  7. GZipJsonBodyDeserializer (application/json and gzip)

## Contribution

The packages uses VSTS pipeline for build and release. The versioning is done by GitVersion.
From all feature (features) branches a new pre-release pacakges will be automatically released.
**After releasing a stable version, the version Tag has to be added to the code with the released version number.**

  