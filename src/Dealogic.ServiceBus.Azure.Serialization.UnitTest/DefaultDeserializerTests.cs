namespace Dealogic.ServiceBus.Azure.Serialization.UnitTest
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DefaultDeserializerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DefaultDeserializerObject_IsNullMessage()
        {
            BodyReader.Default.ReadBody(null, typeof(object));
        }
    }
}