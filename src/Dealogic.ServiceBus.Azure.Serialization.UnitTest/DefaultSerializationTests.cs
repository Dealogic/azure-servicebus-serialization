namespace Dealogic.ServiceBus.Azure.Serialization.UnitTest
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DefaultSerializationTests
    {
        [TestMethod]
        public void TestSerialization_ValidInput()
        {
            foreach (var serializer in GetBodySerializers())
            {
                var mockObject = new MockClass
                {
                    Id = 100,
                    CreatedOn = DateTimeOffset.Parse("2017.12.30", CultureInfo.InvariantCulture)
                };

                var writer = new BodyWriter
                {
                    DefaultBodySerializer = serializer
                };

                var message = new Message();
                writer.WriteBody(message, mockObject);

                Assert.AreEqual(message.UserProperties[CustomPropertyNames.MessageTypeUserPropertyName], mockObject.GetType().FullName);
                Assert.AreEqual(message.ContentType, serializer.ContentType.MediaType);

                var actual = BodyReader.Default.ReadBody(message, mockObject.GetType());
                Assert.AreEqual(mockObject, actual);
            }
        }

        [TestMethod]
        public void TestSerialization_NullInput()
        {
            foreach (var serializer in GetBodySerializers())
            {
                object mockObject = null;

                var writer = new BodyWriter
                {
                    DefaultBodySerializer = serializer
                };

                var message = new Message();
                writer.WriteBody(message, mockObject);

                Assert.IsFalse(message.UserProperties.ContainsKey(CustomPropertyNames.MessageTypeUserPropertyName));
                Assert.IsNull(message.ContentType);

                var actual = BodyReader.Default.ReadBody(message, typeof(object));
                Assert.AreEqual(mockObject, actual);
            }
        }

        [TestMethod]
        public void DefaultSerializeObject_NullMessage()
        {
            BodyWriter.Default.WriteBody(null, null);
        }

        private static IEnumerable<IBodySerializer> GetBodySerializers()
        {
            yield return new BsonBodySerializer();
            yield return new GZipBsonBodySerializer();
            yield return new JsonBodySerializer();
            yield return new GZipJsonBodySerializer();
            yield return new XmlBodySerializer();
            yield return new GZipXmlBodySerializer();
            yield return new BinarySerializer();
        }

        [Serializable]
        [DataContract]
        private class MockClass
        {
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTimeOffset CreatedOn { get; set; }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode() ^ this.CreatedOn.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return this.GetHashCode() == obj.GetHashCode();
            }
        }
    }
}