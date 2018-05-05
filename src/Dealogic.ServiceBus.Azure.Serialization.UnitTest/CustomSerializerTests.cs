namespace Dealogic.ServiceBus.Azure.Serialization.UnitTest
{
    using System;
    using System.Net.Mime;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class CustomSerializerTests
    {
        [TestMethod]
        public void UseCustomSerializer()
        {
            var mockObject = new
            {
                Id = 100
            };

            var mockBodySerializer = Substitute.For<IBodySerializer>();
            mockBodySerializer.ContentType.Returns(new ContentType("application/fake"));
            mockBodySerializer.Serialize(mockObject).Returns(new byte[] { 200 });

            var message = new Message();
            BodyWriter.Default.WriteBody(message, mockObject, mockBodySerializer);

            Assert.AreEqual("application/fake", message.ContentType);
            Assert.AreEqual(200, message.Body[0]);
        }

        [TestMethod]
        public void UseCustomDeserializer()
        {
            var mockObject = new
            {
                Id = 100
            };

            var mockDeserialized = new
            {
                Id = 100
            };

            var mockBodySerializer = Substitute.For<IBodySerializer>();
            mockBodySerializer.ContentType.Returns(new ContentType("application/fake"));
            mockBodySerializer.Serialize(mockObject).Returns(new byte[] { 200 });

            var mockBodyDeserializer = Substitute.For<IBodyDeserializer>();
            mockBodyDeserializer.ContentType.Returns(new ContentType("application/fake"));
            mockBodyDeserializer.Deserialize(Arg.Any<byte[]>(), mockObject.GetType()).Returns(mockDeserialized);

            var message = new Message();
            BodyWriter.Default.WriteBody(message, mockObject, mockBodySerializer);

            var bodyReader = new BodyReader();
            bodyReader.RegisterDeserializer(mockBodyDeserializer);
            var deserializedBody = bodyReader.ReadBody(message, mockObject.GetType());

            Assert.AreEqual(mockObject, deserializedBody);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NoCustomDeserializerRegistered()
        {
            var message = new Message
            {
                ContentType = "application/fake",
                Body = new byte[0]
            };

            BodyReader.Default.ReadBody<object>(message);
        }
    }
}